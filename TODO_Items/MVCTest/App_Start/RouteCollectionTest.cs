using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;

namespace MVCTest.App_Start
{
    public class RouteCollectionTest
    {
        private VirtualPathProvider _vpp;
        private RouteCollection _routeCollection;

        private ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

        #region Props

        public RouteCollection RouteCollection
        {
            get
            {
                if (_routeCollection == null)
                {
                    _routeCollection = RouteTable.Routes;
                }
                return _routeCollection;
            }
            set
            {
                _routeCollection = value;
            }
        }
        public int Count
        {
            get
            {
                return _routeCollection.Count;
            }
        }

        private VirtualPathProvider VPP
        {
            get
            {
                if (_vpp == null)
                {
                    return HostingEnvironment.VirtualPathProvider;
                }
                return _vpp;
            }
            set
            {
                _vpp = value;
            }
        }

        public bool RouteExistingFiles { get { return _routeCollection.RouteExistingFiles; } }

        #endregion

        #region Helpers

        private IDisposable GetReadLock()
        {
            _rwLock.EnterReadLock();
            return ReadLockDisposable(_rwLock);
        }

        #endregion

        public virtual RouteData PostResolveRequestCache(HttpContextBase httpContext)
        {
            bool isRouteToExistingFile = false;
            bool doneRouteCheck = false; // We only want to do the route check once
            if (!RouteExistingFiles)
            {
                isRouteToExistingFile = IsRouteToExistingFile(httpContext);
                doneRouteCheck = true;
                if (isRouteToExistingFile)
                {
                    // If we're not routing existing files and the file exists, we stop processing routes
                    return null;
                }
            }

            // Go through all the configured routes and find the first one that returns a match
            using (GetReadLock())
            {
                foreach (Route route in _routeCollection)
                {
                    RouteData routeData = route.GetRouteDataTest(httpContext);

                    if (routeData != null)
                    {
                        // If we're not routing existing files on this route and the file exists, we also stop processing routes
                        if (!route.RouteExistingFiles)
                        {
                            if (!doneRouteCheck)
                            {
                                isRouteToExistingFile = IsRouteToExistingFile(httpContext);
                                doneRouteCheck = true;
                            }
                            if (isRouteToExistingFile)
                            {
                                return null;
                            }
                        }
                    }

                    return routeData;
                }
            }

            return null;
        }


        // Returns true if this is a request to an existing file
        private bool IsRouteToExistingFile(HttpContextBase httpContext)
        {
            string requestPath = httpContext.Request.AppRelativeCurrentExecutionFilePath;
            return ((requestPath != "~/") &&
                (VPP != null) &&
                (VPP.FileExists(requestPath) ||
                VPP.DirectoryExists(requestPath)));
        }

        private class ReadLockDisposable : IDisposable
        {
            private ReaderWriterLockSlim _rwLock;

            public ReadLockDisposable(ReaderWriterLockSlim rwLock)
            {
                _rwLock = rwLock;
            }

            void IDisposable.Dispose()
            {
                _rwLock.ExitReadLock();
            }
        }
    }

    public static class RouteExtensions
    {
        private static ParsedRoute _parsedRoute;

        public static RouteData GetRouteDataTest(this Route route, HttpContextBase httpContext)
        {
            // Parse incoming URL (we trim off the first two chars since they're always "~/")
            string requestPath = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2) + httpContext.Request.PathInfo;

            _parsedRoute = RouteParser.Parse(route.Url);
            RouteValueDictionary values = _parsedRoute.Match(requestPath, route.Defaults);
            if (values == null)
            {
                // If we got back a null value set, that means the URL did not match
                return null;
            }

            RouteData routeData = new RouteData(route, route.RouteHandler);
            return null;
        }
    }

    internal static class RouteParser
    {
        private static string GetLiteral(string segmentLiteral)
        {
            // Scan for errant single { and } and convert double {{ to { and double }} to }

            // First we eliminate all escaped braces and then check if any other braces are remaining
            string newLiteral = segmentLiteral.Replace("{{", "").Replace("}}", "");
            if (newLiteral.Contains("{") || newLiteral.Contains("}"))
            {
                return null;
            }

            // If it's a valid format, we unescape the braces
            return segmentLiteral.Replace("{{", "{").Replace("}}", "}");
        }
        private static int IndexOfFirstOpenParameter(string segment, int startIndex)
        {
            // Find the first unescaped open brace
            while (true)
            {
                startIndex = segment.IndexOf('{', startIndex);
                if (startIndex == -1)
                {
                    // If there are no more open braces, stop
                    return -1;
                }
                if ((startIndex + 1 == segment.Length) ||
                    ((startIndex + 1 < segment.Length) && (segment[startIndex + 1] != '{')))
                {
                    // If we found an open brace that is followed by a non-open brace, it's
                    // a parameter delimiter.
                    // It's also a delimiter if the open brace is the last character - though
                    // it ends up being being called out as invalid later on.
                    return startIndex;
                }

                // 注释: {{ 表示 '{'
                // Increment by two since we want to skip both the open brace that
                // we're on as well as the subsequent character since we know for
                // sure that it is part of an escape sequence.
                startIndex += 2;
            }
        }

        internal static bool IsSeparator(string s)
        {
            return String.Equals(s, "/", StringComparison.Ordinal);
        }
        private static bool IsValidParameterName(string parameterName)
        {
            if (parameterName.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < parameterName.Length; i++)
            {
                char c = parameterName[i];
                if (c == '/' || c == '{' || c == '}')
                {
                    return false;
                }
            }

            return true;
        }

        internal static IList<string> SplitUrlToPathSegmentStrings(string url)
        {
            List<string> parts = new List<string>();

            if (String.IsNullOrEmpty(url))
            {
                return parts;
            }

            int currentIndex = 0;

            // Split the incoming URL into individual parts
            while (currentIndex < url.Length)
            {
                int indexOfNextSeparator = url.IndexOf('/', currentIndex);
                if (indexOfNextSeparator == -1)
                {
                    // If there are no more separators, the rest of the string is the last part
                    string finalPart = url.Substring(currentIndex);
                    if (finalPart.Length > 0)
                    {
                        parts.Add(finalPart);
                    }
                    break;
                }

                string nextPart = url.Substring(currentIndex, indexOfNextSeparator - currentIndex);
                if (nextPart.Length > 0)
                {
                    parts.Add(nextPart);
                }
                Debug.Assert(url[indexOfNextSeparator] == '/', "The separator char itself should always be a '/'.");
                parts.Add("/");
                currentIndex = indexOfNextSeparator + 1;
            }

            return parts;
        }


        private static IList<PathSubsegment> ParseUrlSegment(string segment, out Exception exception)
        {
            int startIndex = 0;

            List<PathSubsegment> pathSubsegments = new List<PathSubsegment>();

            while (startIndex < segment.Length)
            {
                int nextParameterStart = IndexOfFirstOpenParameter(segment, startIndex);
                if (nextParameterStart == -1)
                {
                    // If there are no more parameters in the segment, capture the remainder as a literal and stop
                    string lastLiteralPart = GetLiteral(segment.Substring(startIndex));
                    if (lastLiteralPart == null)
                    {
                        exception = new ArgumentException(
                            String.Format(
                                CultureInfo.CurrentUICulture,
                                "SR.Route_MismatchedParameter",
                                segment
                            ),
                            "routeUrl");
                        return null;
                    }
                    if (lastLiteralPart.Length > 0)
                    {
                        pathSubsegments.Add(new LiteralSubsegment(lastLiteralPart));
                    }
                    break;
                }

                int nextParameterEnd = segment.IndexOf('}', nextParameterStart + 1);
                if (nextParameterEnd == -1)
                {
                    exception = new ArgumentException(
                        String.Format(
                            CultureInfo.CurrentUICulture,
                            "SR.Route_MismatchedParameter",
                            segment
                        ),
                        "routeUrl");
                    return null;
                }

                string literalPart = GetLiteral(segment.Substring(startIndex, nextParameterStart - startIndex));
                if (literalPart == null)
                {
                    exception = new ArgumentException(
                        String.Format(
                            CultureInfo.CurrentUICulture,
                            "SR.Route_MismatchedParameter",
                            segment
                        ),
                        "routeUrl");
                    return null;
                }
                if (literalPart.Length > 0)
                {
                    pathSubsegments.Add(new LiteralSubsegment(literalPart));
                }

                string parameterName = segment.Substring(nextParameterStart + 1, nextParameterEnd - nextParameterStart - 1);
                pathSubsegments.Add(new ParameterSubsegment(parameterName));

                startIndex = nextParameterEnd + 1;
            }

            exception = null;
            return pathSubsegments;
        }
        private static IList<PathSegment> SplitUrlToPathSegments(IList<string> urlParts)
        {
            List<PathSegment> pathSegments = new List<PathSegment>();
            foreach (string pathSegment in urlParts)
            {
                bool isCurrentPartSeparator = IsSeparator(pathSegment);
                if (isCurrentPartSeparator)
                {
                    pathSegments.Add(new SeparatorPathSegment());
                }
                else
                {
                    Exception exception;
                    IList<PathSubsegment> subsegments = ParseUrlSegment(pathSegment, out exception);
                    Debug.Assert(exception == null, "This only gets called after the path has been validated, so there should never be an exception here");
                    pathSegments.Add(new ContentPathSegment(subsegments));
                }
            }

            return pathSegments;
        }

        private static Exception ValidateUrlParts(IList<string> pathSegments)
        {
            Debug.Assert(pathSegments != null, "The value should always come from SplitUrl(), and that function should never return null.");

            HashSet<string> usedParameterNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            bool? isPreviousPartSeparator = null;

            bool foundCatchAllParameter = false;

            foreach (string pathSegment in pathSegments)
            {
                if (foundCatchAllParameter)
                {
                    // If we ever start an iteration of the loop and we've already found a
                    // catchall parameter then we have an invalid URL format.
                    return new ArgumentException(
                        String.Format(
                            CultureInfo.CurrentCulture,
                            "Route_CatchAllMustBeLast"
                        ),
                        "routeUrl");
                }

                bool isCurrentPartSeparator;
                if (isPreviousPartSeparator == null)
                {
                    // Prime the loop with the first value
                    isPreviousPartSeparator = IsSeparator(pathSegment);
                    isCurrentPartSeparator = isPreviousPartSeparator.Value;
                }
                else
                {
                    isCurrentPartSeparator = IsSeparator(pathSegment);
                    // If both the previous part and the current part are separators, it's invalid
                    if (isCurrentPartSeparator && isPreviousPartSeparator.Value)
                    {
                        return new ArgumentException("Route_CannotHaveConsecutiveSeparators", "routeUrl");
                    }

                    Debug.Assert(isCurrentPartSeparator != isPreviousPartSeparator.Value, "This assert should only happen if both the current and previous parts are non-separators. This should never happen because consecutive non-separators are always parsed as a single part.");
                    isPreviousPartSeparator = isCurrentPartSeparator;
                }


                // If it's not a separator, parse the segment for parameters and validate it
                if (!isCurrentPartSeparator)
                {
                    Exception exception;
                    IList<PathSubsegment> subsegments = ParseUrlSegment(pathSegment, out exception);
                    if (exception != null)
                    {
                        return exception;
                    }

                    exception = ValidateUrlSegment(subsegments, usedParameterNames, pathSegment);
                    if (exception != null)
                    {
                        return exception;
                    }

                    foundCatchAllParameter = subsegments.Any<PathSubsegment>(seg => (seg is ParameterSubsegment) && (((ParameterSubsegment)seg).IsCatchAll));
                }

            }

            return null;
        }

        private static Exception ValidateUrlSegment(IList<PathSubsegment> pathSubsegments, HashSet<string> usedParameterNames, string pathSegment)
        {
            bool segmentContainsCatchAll = false;
            Type previousSegmentType = null;
            foreach (PathSubsegment subsegment in pathSubsegments)
            {
                if (previousSegmentType != null)
                {
                    if (previousSegmentType == subsegment.GetType())
                    {
                        return new ArgumentException(
                            String.Format(
                                CultureInfo.CurrentCulture,
                                "SR.Route_CannotHaveConsecutiveParameters"
                            ),
                            "routeUrl");
                    }
                }
                previousSegmentType = subsegment.GetType();
                // note: 只有Literal, ParameterSubsegment
                LiteralSubsegment literalSubsegment = subsegment as LiteralSubsegment;
                if (literalSubsegment != null)
                {
                    // Nothing to validate for literals - everything is valid
                }
                else
                {
                    ParameterSubsegment parameterSubsegment = subsegment as ParameterSubsegment;
                    if (parameterSubsegment != null)
                    {
                        string parameterName = parameterSubsegment.ParameterName;
                        if (parameterSubsegment.IsCatchAll)
                        {
                            segmentContainsCatchAll = true;
                        }

                        // Check for valid characters in the parameter name
                        if (!IsValidParameterName(parameterName))
                        {
                            return new ArgumentException(
                                String.Format(
                                    CultureInfo.CurrentUICulture,
                                    "SR.Route_InvalidParameterName",
                                    parameterName
                                ),
                                "routeUrl");
                        }

                        if (usedParameterNames.Contains(parameterName))
                        {
                            return new ArgumentException(
                                String.Format(
                                    CultureInfo.CurrentUICulture,
                                    "SR.Route_RepeatedParameter",
                                    parameterName
                                ),
                                "routeUrl");
                        }
                        else
                        {
                            usedParameterNames.Add(parameterName);
                        }
                    }
                    else
                    {
                        Debug.Fail("Invalid path subsegment type");
                    }
                }
            }

            if (segmentContainsCatchAll && (pathSubsegments.Count != 1))
            {
                return new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        "SR.Route_CannotHaveCatchAllInMultiSegment"
                    ),
                    "routeUrl");
            }

            return null;
        }

        internal static bool IsInvalidRouteUrl(string routeUrl)
        {
            return (routeUrl.StartsWith("~", StringComparison.Ordinal) ||
                routeUrl.StartsWith("/", StringComparison.Ordinal) ||
                (routeUrl.IndexOf('?') != -1));
        }

        public static ParsedRoute Parse(string routeUrl)
        {
            if (routeUrl == null)
            {
                routeUrl = String.Empty;
            }

            if (IsInvalidRouteUrl(routeUrl))
            {
                throw new ArgumentException("Route_InvalidRouteUrl", "routeUrl");
            }

            IList<string> urlParts = SplitUrlToPathSegmentStrings(routeUrl);
            Exception ex = ValidateUrlParts(urlParts);
            if (ex != null)
            {
                throw ex;
            }

            IList<PathSegment> pathSegments = SplitUrlToPathSegments(urlParts);

            Debug.Assert(urlParts.Count == pathSegments.Count, "The number of string segments should be the same as the number of path segments");

            return new ParsedRoute(pathSegments);
        }
    }

    internal sealed class ParsedRoute
    {
        public ParsedRoute(IList<PathSegment> pathSegments)
        {
            Debug.Assert(pathSegments != null);

            PathSegments = pathSegments;
        }

        private IList<PathSegment> PathSegments
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualPath">当前请求url,  + PathInfo</param>
        /// <param name="defaultValues"></param>
        /// <returns></returns>
        public RouteValueDictionary Match(string virtualPath, RouteValueDictionary defaultValues)
        {
            IList<string> requestPathSegments = RouteParser.SplitUrlToPathSegmentStrings(virtualPath);

            if (defaultValues == null)
            {
                defaultValues = new RouteValueDictionary();
            }

            RouteValueDictionary matchedValues = new RouteValueDictionary();

            // This flag gets set once all the data in the URL has been parsed through, but
            // the route we're trying to match against still has more parts. At this point
            // we'll only continue matching separator characters and parameters that have
            // default values.
            bool ranOutOfStuffToParse = false;

            // This value gets set once we start processing a catchall parameter (if there is one
            // at all). Once we set this value we consume all remaining parts of the URL into its
            // parameter value.
            bool usedCatchAllParameter = false;

            // 检查url跟当前route是否匹配
            for (int i = 0; i < PathSegments.Count; i++)
            {
                PathSegment pathSegment = PathSegments[i];

                if (requestPathSegments.Count <= i)
                {
                    ranOutOfStuffToParse = true;
                }

                string requestPathSegment = ranOutOfStuffToParse ? null : requestPathSegments[i];

                if (pathSegment is SeparatorPathSegment)
                {
                    if (ranOutOfStuffToParse)
                    {
                        // If we're trying to match a separator in the route but there's no more content, that's OK
                    }
                    else
                    {
                        if (!String.Equals(requestPathSegment, "/", StringComparison.Ordinal))
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    ContentPathSegment contentPathSegment = pathSegment as ContentPathSegment;

                    if (contentPathSegment != null)
                    {
                        if (contentPathSegment.IsCatchAll)
                        {
                            Debug.Assert(i == (PathSegments.Count - 1), "If we're processing a catch-all, we should be on the last route segment.");
                            MatchCatchAll(contentPathSegment, requestPathSegments.Skip(i), defaultValues, matchedValues);
                            usedCatchAllParameter = true;
                        }
                        else
                        {
                            if (!MatchContentPathSegment(contentPathSegment, requestPathSegment, defaultValues, matchedValues))
                            {
                                return null;
                            }
                        }
                    }
                    else
                    {
                        Debug.Fail("Invalid path segment type");
                    }
                }
            }

            if (!usedCatchAllParameter)
            {
                if (PathSegments.Count < requestPathSegments.Count)
                {
                    // If we've already gone through all the parts defined in the route but the URL
                    // still contains more content, check that the remaining content is all separators.
                    for (int i = PathSegments.Count; i < requestPathSegments.Count; i++)
                    {
                        if (!RouteParser.IsSeparator(requestPathSegments[i]))
                        {
                            return null;
                        }
                    }
                }
            }
            // Copy all remaining default values to the route data
            if (defaultValues != null)
            {
                foreach (var defaultValue in defaultValues)
                {
                    if (!matchedValues.ContainsKey(defaultValue.Key))
                    {
                        matchedValues.Add(defaultValue.Key, defaultValue.Value);
                    }
                }
            }

            return matchedValues;
        }

        private void MatchCatchAll(ContentPathSegment contentPathSegment, IEnumerable<string> remainingRequestSegments, RouteValueDictionary defaultValues, RouteValueDictionary matchedValues)
        {
            string remainingRequest = String.Join(String.Empty, remainingRequestSegments.ToArray());

            ParameterSubsegment catchAllSegment = contentPathSegment.Subsegments[0] as ParameterSubsegment;

            object catchAllValue;

            if (remainingRequest.Length > 0)
            {
                catchAllValue = remainingRequest;
            }
            else
            {
                defaultValues.TryGetValue(catchAllSegment.ParameterName, out catchAllValue);
            }
            matchedValues.Add(catchAllSegment.ParameterName, catchAllValue);
        }

        private bool MatchContentPathSegment(ContentPathSegment routeSegment, string requestPathSegment, RouteValueDictionary defaultValues, RouteValueDictionary matchedValues)
        {
            if (String.IsNullOrEmpty(requestPathSegment))
            { 
                // If there's no data to parse, we must have exactly one parameter segment and no other segments - otherwise no match
                if (routeSegment.Subsegments.Count > 1) {
                    return false;
                }


            }
            
            // Find last literal segment and get its last index in the string
            
        }
    }
}