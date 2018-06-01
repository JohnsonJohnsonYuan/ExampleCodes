using System;
using System.Collections.Generic;
using System.Linq;

namespace MVCTest.App_Start
{
    internal abstract class PathSegment
    {
#if ROUTE_DEBUGGING
        public abstract string LiteralText {
            get;
        }
#endif
    }

    internal abstract class PathSubsegment
    {
#if ROUTE_DEBUGGING
        public abstract string LiteralText {
            get;
        }
#endif
    }

    // Represents a "/" separator in a URL
    internal sealed class SeparatorPathSegment : PathSegment
    {
#if ROUTE_DEBUGGING
        public override string LiteralText {
            get {
                return "/";
            }
        }
 
        public override string ToString() {
            return "\"/\"";
        }
#endif
    }

    internal sealed class ContentPathSegment : PathSegment
    {
        public ContentPathSegment(IList<PathSubsegment> subsegments)
        {
            Subsegments = subsegments;
        }

        public bool IsCatchAll
        {
            get
            {
                // 
                return Subsegments.Any<PathSubsegment>(seg => (seg is ParameterSubsegment) && (((ParameterSubsegment)seg).IsCatchAll));
            }
        }

        public IList<PathSubsegment> Subsegments
        {
            get;
            private set;
        }

#if ROUTE_DEBUGGING
        public override string LiteralText {
            get {
                List<string> s = new List<string>();
                foreach (PathSubsegment subsegment in Subsegments) {
                    s.Add(subsegment.LiteralText);
                }
                return String.Join(String.Empty, s.ToArray());
            }
        }
 
        public override string ToString() {
            List<string> s = new List<string>();
            foreach (PathSubsegment subsegment in Subsegments) {
                s.Add(subsegment.ToString());
            }
            return "[ " + String.Join(", ", s.ToArray()) + " ]";
        }
#endif
    }

    internal sealed class ParameterSubsegment : PathSubsegment
    {
        public ParameterSubsegment(string parameterName)
        {
            if (parameterName.StartsWith("*", StringComparison.Ordinal))
            {
                ParameterName = parameterName.Substring(1);
                IsCatchAll = true;
            }
            else
            {
                ParameterName = parameterName;
            }
        }

        public bool IsCatchAll
        {
            get;
            private set;
        }

        public string ParameterName
        {
            get;
            private set;
        }

#if ROUTE_DEBUGGING
        public override string LiteralText {
            get {
                return "{" + (IsCatchAll ? "*" : String.Empty) + ParameterName + "}";
            }
        }
 
        public override string ToString() {
            return "{" + (IsCatchAll ? "*" : String.Empty) + ParameterName + "}";
        }
#endif
    }

    internal sealed class LiteralSubsegment : PathSubsegment
    {
        public LiteralSubsegment(string literal)
        {
            Literal = literal;
        }

        public string Literal
        {
            get;
            private set;
        }

#if ROUTE_DEBUGGING
        public override string LiteralText {
            get {
                return Literal;
            }
        }
 
        public override string ToString() {
            return "\"" + Literal + "\"";
        }
#endif
    }
}