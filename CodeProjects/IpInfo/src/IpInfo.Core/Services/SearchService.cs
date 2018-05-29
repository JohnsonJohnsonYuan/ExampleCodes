using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IpInfo.Core.Models;
using System.Diagnostics;

namespace IpInfo.Core.Services
{
    /// <summary>
    /// 搜索结果
    /// </summary>
    public class SearchRecord
    {
        public SearchRecord()
        {
            Progress = new List<string>();
        }
        public IpRecord IpRecord { get; set; }
        public int Count { get; set; }
        public int Index { get; set; }
        public List<string> Progress { get; set; }
        public double ElapsedMilliseconds { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}\t{1}\t{2}\t", Count, Index, ElapsedMilliseconds);
            if (Progress.Count > 0)
            {
                var progress = Progress.Aggregate((x, y) => x + ", " + y);
                sb.Append(progress);
            }
            return sb.ToString();
        }

        public static string FormatProgress(int low, int high)
        {
            if (low == high)
                return string.Format("[{0}]", low);
            return string.Format("[{0}, {1}]", low, high);
        }
    }

    public class SearchService
    {
        /// <summary>
        /// * 二分查找算法:
        /// * 要求:有序,线性结构
        /// * 时间复杂度O[log(n)]
        /// * 循环实现
        /// * middle = (low + high) / 2;
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="des"></param>
        /// <returns></returns>
        public int Binary_Search(IpRecord[] arr, long key, out SearchRecord recorder)
        {
            recorder = new SearchRecord();

            int search_count = 0;

            int low = 0;
            int high = arr.Length - 1;
            while (low <= high)
            {
                ++search_count;

                int middle = (low + high) / 2;
                //先拿中间的值与查找的值比较,大于中间的值,说明查找的值索引在[low,middle)

                recorder.Progress.Add(SearchRecord.FormatProgress(middle, middle));
                if (arr[middle].CompareTo(key) == 0)
                {
                    recorder.IpRecord = arr[middle];
                    recorder.Count = search_count;
                    recorder.Index = middle;

                    return middle;
                }
                else if (arr[middle].CompareTo(key) > 0)   // > key
                {
                    high = middle - 1;

                    recorder.Progress.Add("*" + SearchRecord.FormatProgress(low, high));
                }
                else
                {
                    low = middle + 1;

                    recorder.Progress.Add("*" + SearchRecord.FormatProgress(low, high));
                }
            }

            recorder.Index = -1;
            return -1;
        }

        #region 斐波那契 查找 3个版本

        static int[] Fib = { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765,
                                 10946, 17711, 28657, 46368, 75025, 121393, 196418, 317811, 514229, 832040, 1346269, 2178309, 3524578,
                                 5702887, 9227465, 14930352, 24157817, 39088169, 63245986, 102334155, 165580141, 267914296,
                                 433494437, 701408733, 1134903170, 1836311903, Int32.MaxValue
                                };

        /// <summary>
        /// * 斐波那契 查找:
        /// * 要求:有序,线性结构
        /// * 时间复杂度O[log(n)]
        /// * middle = low + Fib[k - 1] - 1;
        /// 
        /// 修改部分：
        /// 传入数组之后不创建临时数组， 根据数组长度在斐波那契数组中index值，
        /// 判断 if (mid >= arr.Length) { 
        ///     high = mid - 1; fibIndex = fibIndex - 1;
        /// } else {
        ///     // 原来判断方法
        /// }
        /// </summary>
        /// <param name="arr">需要查找的原数组</param>
        /// <param name="key">查找的元素</param>
        /// <param name="recorder"></param>
        /// <param name="fibIndex">数组长度在斐波那契数组中index值， 如果不提供， 会自动计算</param>
        /// <returns></returns>
        public int Fibonacci_Search_Improved(IpRecord[] arr, long key, out SearchRecord recorder, int fibIndex = -1)
        {
            recorder = new SearchRecord();

            int search_count = 0;

            int low = 0;
            int high = arr.Length - 1;
            int mid = 0;

            //斐波那契分割数值下标
            if (fibIndex < 0)
            {
                int k = 0;

                //获取斐波那契分割数值下标
                while (arr.Length > Fib[k] - 1)
                {
                    k++;
                }

                fibIndex = k;
            }

            while (low <= high)
            {
                ++search_count;

                // low：起始位置  
                // 前半部分有f[k-1]个元素，由于下标从0开始 
                // 则-1 获取 黄金分割位置元素的下标
                mid = low + Fib[fibIndex - 1] - 1;

                recorder.Progress.Add(SearchRecord.FormatProgress(mid, mid));

                if (mid >= arr.Length)
                {
                    // 如果超出数组长度， 选哟查找的元素肯定在分割点左侧
                    // 这样判断就不需要新建和补齐临时数组了
                    high = mid - 1;

                    fibIndex = fibIndex - 1;

                    recorder.Progress.Add(SearchRecord.FormatProgress(low, high));
                }
                else
                {
                    if (arr[mid].CompareTo(key) > 0)    // > key
                    {
                        // 查找前半部分，高位指针移动
                        high = mid - 1;
                        // （全部元素） = （前半部分）+（后半部分）
                        //   f[k]  =  f[k-1] + f[k-1]
                        // 因为前半部分有f[k-1]个元素，所以 k = k-1  
                        fibIndex = fibIndex - 1;

                        recorder.Progress.Add(SearchRecord.FormatProgress(low, high));
                    }
                    else if (arr[mid].CompareTo(key) < 0)   // arr[mid] < key
                    {
                        // 查找后半部分，高位指针移动
                        low = mid + 1;
                        // （全部元素） = （前半部分）+（后半部分）
                        //   f[k]  =  f[k-1] + f[k-1]
                        // 因为后半部分有f[k-2]个元素，所以 k = k-2 
                        fibIndex = fibIndex - 2;

                        recorder.Progress.Add(SearchRecord.FormatProgress(low, high));
                    }
                    else
                    {
                        recorder.IpRecord = arr[mid];
                        recorder.Count = search_count;

                        recorder.Index = mid;
                        return mid;
                    }
                }
            }

            recorder.Index = -1;
            return -1;
        }

        #endregion
    }
}
