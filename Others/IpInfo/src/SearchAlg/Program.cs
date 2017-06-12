using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SearchAlg
{
    /// <summary>
    /// 查找次数和用时(ms)比较
    /// 插值查找效率跟数组元素值有关, 有时候分隔值可能1， 2，3， 。。。。 这样一个一个增加， 速度很慢
    /// 
    /// 虽然斐波那契查找用的次数平均比二分法多 ，但是查找效率要高（只有加减法来计算分分隔点）
    /// 
    /// 结论： 斐波那契查找较好
    /// 
    /// Array Length        binary      fbonacci
    ///       9             2, 0.013    4, 0.005
    ///     254             5, 0.0358   10, 0.01
    /// </summary>
    class Program
    {
        #region Utilities

        /// <summary>
        /// 生成随机数组查询
        /// </summary>
        /// <param name="arrayLength"></param>
        /// <param name="resultFilePath"></param>
        /// <param name="appendResult"></param>
        static void TestSearch(int arrayLength, string resultFilePath, bool appendResult = true)
        {
            var array = CommonHelper.GenerateRandomArray(arrayLength);
            Array.Sort(array);

            var key = CommonHelper.GetArrayRandomItem(array);

            Stopwatch st = new Stopwatch();

            st.Start();
            SearchRecord binary_recorder;
            Binary_Search(array, key, out binary_recorder);
            st.Stop();
            binary_recorder.ElapsedTicks = st.Elapsed.TotalMilliseconds;

            //st.Restart();
            //SearchRecord insertValue_recorder;
            //InsertValue_Search(array, key, out insertValue_recorder);
            //st.Stop();
            //insertValue_recorder.ElapsedTicks = st.ElapsedTicks;

            st.Restart();
            SearchRecord fbonacci_recorder;
            Fibonacci_Search_Improved(array, key, out fbonacci_recorder);
            st.Stop();
            fbonacci_recorder.ElapsedTicks = st.Elapsed.TotalMilliseconds;

            using (StreamWriter writer = new StreamWriter(resultFilePath, appendResult))
            {
                writer.WriteLine(string.Format("Array Length: {0}, Key: {1}", array.Length, key));
                int i = 0;
                foreach (var item in array)
                {
                    if (i > 30)
                    {
                        writer.Write("...");
                        break;
                    }

                    writer.Write(item);
                    ++i;
                    if (i != array.Length)
                        writer.Write(", ");
                }
                writer.WriteLine();

                writer.Write("binary\t");
                writer.WriteLine(binary_recorder);
                //writer.Write("insertValue\t");
                //writer.WriteLine(insertValue_recorder);
                writer.Write("fbonacci\t");
                writer.WriteLine(fbonacci_recorder);

                writer.WriteLine();
                writer.WriteLine();
            }
        }


        /// <summary>
        /// 数组长度是5000000元素为1到5000000的数组进行查找
        /// </summary>
        private static void TestSearch()
        {
            // 1到5000000
            int[] array = new int[5000000];
            for (int i = 0; i < 5000000; i++)
            {
                array[i] = i + 1;
            }

            SearchRecord binary_recorder;
            SearchRecord insert_recorder;
            SearchRecord fib_recorder;

            // List<string> results = new List<string>();
            List<CompareResult> results = new List<CompareResult>();

            using (StreamWriter writer = new StreamWriter("test.txt", true))
            {
                writer.WriteLine(string.Format("\t\t{0}\t{1}\t{2}", "Binary", "Insert", "Fibonacci"));
            }

            Stopwatch st = new Stopwatch();

            st.Start();

            #region 斐波那契数组

            //斐波那契分割数值下标
            int fibIndex = 0;

            int high = array.Length - 1;
            //获取斐波那契分割数值下标
            while (array.Length > Fib[fibIndex] - 1)
            {
                fibIndex++;
            }

            //创建临时数组
            int[] fibArray = new int[Fib[fibIndex] - 1];
            Array.Copy(array, fibArray, array.Length);

            st.Stop();
            Console.WriteLine("新建长度是" + fibArray.Length + " 用时 " + st.Elapsed.TotalMilliseconds);

            st.Restart();
            //序列补充至f[k]个元素
            //补充的元素值为最后一个元素的值
            for (int i = array.Length; i < fibArray.Length; i++)
            {
                fibArray[i] = array[high];
            }

            st.Stop();
            Console.WriteLine("准备斐波那契数组用时： " + st.Elapsed.TotalMilliseconds);

            //return;
            #endregion

            for (int i = 0; i < array.Length; )
            {
                var item = array[i];

                Console.WriteLine(item);

                st.Restart();
                Binary_Search(array, item, out binary_recorder);
                st.Stop();
                binary_recorder.ElapsedTicks = st.Elapsed.TotalMilliseconds;


                st.Restart();
                Fibonacci_Search_Improved(array, item, out insert_recorder, fibIndex);
                st.Stop();
                insert_recorder.ElapsedTicks = st.Elapsed.TotalMilliseconds;


                st.Restart();
                // Fibonacci_Search(array, item, out fib_recorder);
                Fibonacci_Search(fibArray, item, out fib_recorder, fibIndex);
                st.Stop();
                fib_recorder.ElapsedTicks = st.Elapsed.TotalMilliseconds;



                using (StreamWriter writer = new StreamWriter("test.txt", true))
                {
                    var content = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", item,
                    binary_recorder.ElapsedTicks + " (" + binary_recorder.Count + ")",
                    insert_recorder.ElapsedTicks + " (" + insert_recorder.Count + ")",
                    fib_recorder.ElapsedTicks + " (" + fib_recorder.Count + ")",

                    binary_recorder.Index + " - " + insert_recorder.Index + " - " + fib_recorder.Index);
                    writer.WriteLine(content);
                }

                if (i > 0)
                    i = (int)Math.Ceiling(i * 1.2);
                else
                    i++;

                CompareResult result = new CompareResult();
                result.Binary_record = binary_recorder;
                result.Insert_record = insert_recorder;
                result.Fbonacci_record = fib_recorder;

                results.Add(result);
                //results.Add(string.Format("{0}\t{1}\t{2}\t{3}\t{4}", item,
                //    binary_recorder.ElapsedTicks,
                //    insert_recorder.ElapsedTicks,
                //    fib_recorder.ElapsedTicks,
                //    binary_recorder.Index + " - " + insert_recorder.Index + " - " + fib_recorder.Index));
            }

            var sb = new StringBuilder();
            using (var tw = new StringWriter(sb))
            {
                var xmlS = new XmlSerializer(typeof(List<CompareResult>));
                xmlS.Serialize(tw, results);
            }

            File.WriteAllText("serialize.txt", sb.ToString());
        }
        #endregion

        static void Main(string[] args)
        {
            //TestSearch();
            //return;

            SearchRecord binary_recorder;
            int[] arr2 = new int[100];
            for (int i = 0; i < 100; i++)
            {
                arr2[i] = 172936 + i + new Random().Next(1,15);
            }
            int[] arr = {
                          0, 1, 16, 24, 35, 47, 59, 62, 73, 88, 99
                      };


            InsertValue_Search(arr, 59, out binary_recorder);
            Console.WriteLine(binary_recorder);
            Binary_Search(arr, 59, out binary_recorder);
            Console.WriteLine(binary_recorder);
            //Fibonacci_Search(arr, 59, out binary_recorder);
            Console.WriteLine(binary_recorder);


            foreach (var item in arr2)
            {
                InsertValue_Search(arr2, item, out binary_recorder); 
                Console.WriteLine(binary_recorder);
            }
           // 
            //return;

            const int testCount = 3;
            Random rand = new Random();
            var tmpLength = rand.Next(10);
            for (int i = 0; i < testCount; i++)
            {
                TestSearch(tmpLength, "result.txt");
            }

            int minLength = 10;
            int maxLength = 100;

            for (long j = 1; j < Int32.MaxValue; )
            {
                for (int i = 0; i < testCount; i++)
                {
                    Console.WriteLine(j + " " + tmpLength);
                    tmpLength = CommonHelper.GenerateRandomInteger(minLength, maxLength);
                    TestSearch(tmpLength, "result.txt");
                }
                j += 200;

                minLength += 200;
                maxLength += 200;
            }
        }

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
        static int Binary_Search(int[] arr, int key, out SearchRecord recorder)
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

                if (key == arr[middle])
                {
                    recorder.Count = search_count;
                    recorder.Index = middle;

                    return middle;
                }
                else if (key < arr[middle])
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

        /// <summary>
        /// * 插值查找算法:
        /// * 要求:有序,线性结构
        /// * 时间复杂度O[log(n)]
        /// * middle = low + (key - arr[low]) / (arr[high] - arr[low]) * (high - low)
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="des"></param>
        /// <returns></returns>
        static int InsertValue_Search(int[] arr, int key, out SearchRecord recorder)
        {
            recorder = new SearchRecord();

            int search_count = 0;

            int low = 0;
            int high = arr.Length - 1;
            while (low <= high)
            {
                ++search_count;

                int middle = low + (key - arr[low]) / (arr[high] - arr[low]) * (high - low);

                // recorder.Progress.Add(SearchRecord.FormatProgress(middle, middle));

                //先拿中间的值与查找的值比较,大于中间的值,说明查找的值索引在[low,middle)
                if (key == arr[middle])
                {
                    recorder.Index = middle;
                    recorder.Count = search_count;

                    return middle;
                }
                else if (key < arr[middle])
                {
                    high = middle - 1;

                    recorder.Progress.Add(SearchRecord.FormatProgress(low, high));
                }
                else
                {
                    low = middle + 1;

                    recorder.Progress.Add(SearchRecord.FormatProgress(low, high));
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
        /// 由于创建数组需要消耗大量时间 (new int[500000] 大约耗时24ms, 但查找过程只需0.03秒左右)
        /// 所以后面两个版本不再函数中创建新的数组
        /// 
        /// 第二种于第三种方法都可以， 耗时差别不大
        /// </summary>
        /// <param name="arr">需要查找的原数组</param>
        /// <param name="key">查找的元素</param>
        /// <returns></returns>
        static int Fibonacci_Search(int[] arr, int key, out SearchRecord recorder)
        {
            recorder = new SearchRecord();

            int search_count = 0;

            int low = 0;
            int high = arr.Length - 1;
            int mid = 0;

            //斐波那契分割数值下标
            int k = 0;
            
            //获取斐波那契分割数值下标
            while (arr.Length > Fib[k] - 1)
            {
                k++;
            }

            //创建临时数组
            int[] temp = new int[Fib[k] - 1];
            Array.Copy(arr, temp, arr.Length);
            //序列补充至f[k]个元素
            //补充的元素值为最后一个元素的值
            for (int i = arr.Length; i < temp.Length; i++)
            {
                temp[i] = arr[high];
            }

#if !DEBUG
            foreach (var item in temp)
            {
                Console.Write(item + " ");
            }
            Console.WriteLine();
#endif

            while (low <= high)
            {
                ++search_count;

                // low：起始位置  
                // 前半部分有f[k-1]个元素，由于下标从0开始 
                // 则-1 获取 黄金分割位置元素的下标
                mid = low + Fib[k - 1] - 1;

                recorder.Progress.Add(SearchRecord.FormatProgress(mid, mid));

                if (temp[mid] > key)
                {
                    // 查找前半部分，高位指针移动
                    high = mid - 1;
                    // （全部元素） = （前半部分）+（后半部分）
                    //   f[k]  =  f[k-1] + f[k-1]
                    // 因为前半部分有f[k-1]个元素，所以 k = k-1  
                    k = k - 1;

                    recorder.Progress.Add(SearchRecord.FormatProgress(low, high));
                }
                else if (temp[mid] < key)
                {
                    // 查找后半部分，高位指针移动
                    low = mid + 1;
                    // （全部元素） = （前半部分）+（后半部分）
                    //   f[k]  =  f[k-1] + f[k-1]
                    // 因为后半部分有f[k-2]个元素，所以 k = k-2 
                    k = k - 2;

                    recorder.Progress.Add(SearchRecord.FormatProgress(low, high));
                }
                else
                {
                    recorder.Count = search_count;

                    //如果为真则找到相应的位置
                    if (mid <= high)
                    {
                        recorder.Index = mid;
                        return mid;
                    }
                    else
                    {
                        //出现这种情况是查找到补充的元素
                        //而补充的元素与high位置的元素一样
                        recorder.Index = high;
                        return high;
                    }
                }
            }

            recorder.Index = -1;
            return -1;
        }

        /// <summary>
        /// * 斐波那契 查找:
        /// * 要求:有序,线性结构
        /// * 时间复杂度O[log(n)]
        /// * middle = low + Fib[k - 1] - 1;
        /// 
        /// 修改部分：
        /// 传入数组是按照斐波那契长度-1且补齐的数组
        /// </summary>
        /// <param name="arr">原数组按照斐波那契长度-1且补齐</param>
        /// <param name="key">查找的元素</param>
        /// <param name="recorder"></param>
        /// <param name="fibIndex">数组长度在斐波那契数组中index值， 如果不提供， 会自动计算</param>
        /// <returns></returns>
        static int Fibonacci_Search(int[] temp, int key, out SearchRecord recorder, int fibIndex = -1)
        {
            recorder = new SearchRecord();

            int search_count = 0;

            int low = 0;
            int high = temp.Length - 1;
            int mid = 0;

            //斐波那契分割数值下标
            if (fibIndex < 0)
            {
                int k = 0;

                //获取斐波那契分割数值下标
                while (temp.Length > Fib[fibIndex] - 1)
                {
                    fibIndex++;
                }
            }

            while (low <= high)
            {
                ++search_count;

                // low：起始位置  
                // 前半部分有f[k-1]个元素，由于下标从0开始 
                // 则-1 获取 黄金分割位置元素的下标
                mid = low + Fib[fibIndex - 1] - 1;

                recorder.Progress.Add(SearchRecord.FormatProgress(mid, mid));

                if (temp[mid] > key)
                {
                    // 查找前半部分，高位指针移动
                    high = mid - 1;
                    // （全部元素） = （前半部分）+（后半部分）
                    //   f[k]  =  f[k-1] + f[k-1]
                    // 因为前半部分有f[k-1]个元素，所以 k = k-1  
                    fibIndex = fibIndex - 1;

                    recorder.Progress.Add(SearchRecord.FormatProgress(low, high));
                }
                else if (temp[mid] < key)
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
                    recorder.Count = search_count;

                    //如果为真则找到相应的位置
                    if (mid <= high)
                    {
                        recorder.Index = mid;
                        return mid;
                    }
                    else
                    {
                        //出现这种情况是查找到补充的元素
                        //而补充的元素与high位置的元素一样
                        recorder.Index = high;
                        return high;
                    }
                }
            }

            recorder.Index = -1;
            return -1;
        }

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
        static int Fibonacci_Search_Improved(int[] arr, int key, out SearchRecord recorder, int fibIndex = -1)
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
                    if (arr[mid] > key)
                    {
                        // 查找前半部分，高位指针移动
                        high = mid - 1;
                        // （全部元素） = （前半部分）+（后半部分）
                        //   f[k]  =  f[k-1] + f[k-1]
                        // 因为前半部分有f[k-1]个元素，所以 k = k-1  
                        fibIndex = fibIndex - 1;

                        recorder.Progress.Add(SearchRecord.FormatProgress(low, high));
                    }
                    else if (arr[mid] < key)
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

    /// <summary>
    /// 搜索结果
    /// </summary>
    public class SearchRecord
    {
        public SearchRecord()
        {
            Progress = new List<string>();
        }

        public int Count { get; set; }
        public int Index { get; set; }
        public List<string> Progress { get; set; }
        public double ElapsedTicks { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}\t{1}\t{2}\t", Count, Index, ElapsedTicks);
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

    public class CompareResult
    {
        public SearchRecord Binary_record { get; set; }
        public SearchRecord Insert_record { get; set; }
        public SearchRecord Fbonacci_record { get; set; }
    }
}
