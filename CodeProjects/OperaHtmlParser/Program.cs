using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace OperaHtmlParser
{
    class Program
    {
        // 从html读取的json信息缓存
        private static string _CACHE_JSON_FILE = "_parseCache.json";
        static Program()
        {
            // http://www.bavc.com.cn 使用gb2312编码

            // 支持gb2312编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            StaticVariables.GB2312Encoding = Encoding.GetEncoding("GB2312");

            System.Console.WriteLine("添加GB2312支持");
        }
        static bool _loaded = false;
        ///<summary>
        /// 北京典籍与经典老唱片数字化出版项目
        /// 2018年出品 http://www.bavc.com.cn/c44167.htm
        /// 2019年出品 http://www.bavc.com.cn/c44417.htm
        ///</summary>
        [STAThread]
        static void Main(string[] args)
        {
            //var url2 = "http://www.bavc.com.cn/w10279097.htm?page=1";

            HtmlParseLogger.TagError("tag 错误");
            Uri uri = new Uri("http://mpv.videocc.net/d69fff2eae/c/d69fff2eaef75ebc9d69ae4e5f3a891c_1.mp4?pid=1578623766586X1882197");
            var query = uri.Query;
            var index = uri.OriginalString.IndexOf(query);
            var newUrl = uri.OriginalString.Substring(0, index);
            System.Console.WriteLine(uri.OriginalString);
            System.Console.WriteLine(newUrl);


            HtmlParseLogger.Info("Start...");

            #region 从缓存文件 或 解析html读取信息

            // 要下载的目录
            Dictionary<string, string> downloadItems = new Dictionary<string, string>
            {
                {"2018年出品-京剧", "http://www.bavc.com.cn/c44167.htm"},
                {"2018年出品-老北京曲艺", "http://www.bavc.com.cn/c44168.htm"},
                {"2018年出品-评剧", "http://www.bavc.com.cn/c44169.htm"},

                {"2019年出品-京剧", "http://www.bavc.com.cn/c44417.htm"},
                {"2019年出品-曲艺", "http://www.bavc.com.cn/c44418.htm"},
            };

            // 转换为model
            List<ExcelSheetModel> sheetModels = null;
            if (System.IO.File.Exists(_CACHE_JSON_FILE))
            {
                try
                {
                    var jsonStr = System.IO.File.ReadAllText(_CACHE_JSON_FILE);
                    sheetModels = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExcelSheetModel>>(jsonStr);

                    System.Console.WriteLine($"从缓存文件{_CACHE_JSON_FILE}中读取记录成功");
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    sheetModels = null;
                }
            }

            // 解析html
            if (sheetModels == null)
            {
                sheetModels = new List<ExcelSheetModel>();

                // 缓存html然后
                foreach (var item in downloadItems)
                {
                    var url = item.Value;
                    if (string.IsNullOrEmpty(url))
                        continue;

                    ExcelSheetModel model = new ExcelSheetModel();
                    model.Name = item.Key;
                    model.MediaItems = ParseMediaItems(url);

                    sheetModels.Add(model);
                }

                // cache
                System.Console.WriteLine($"缓存文件{_CACHE_JSON_FILE}");
                System.IO.File.WriteAllText(
                    _CACHE_JSON_FILE,
                    Newtonsoft.Json.JsonConvert.SerializeObject(sheetModels)
                );
                System.IO.File.WriteAllText(
                    System.IO.Path.GetFileNameWithoutExtension(_CACHE_JSON_FILE) + ".formatted" + System.IO.Path.GetExtension(_CACHE_JSON_FILE),
                    Newtonsoft.Json.JsonConvert.SerializeObject(sheetModels, Newtonsoft.Json.Formatting.Indented)
                );
            }

            #endregion

            #region 手动修改一些错误

            // 根据文件名猜测artist时网页命名出现错误, 原文件名得到的是"林树森 华容道2 金少山"
            // "06 华容道2 林树森 金少山 胜利" 名称修改为 "06.华容道2 林树森 金少山 胜利"
            // 11 京剧红生大王《林树森专辑》
            NormalizeMp3FileName(sheetModels,
                "06 华容道2 林树森 金少山 胜利",
                "06.华容道2 林树森 金少山 胜利");

            // 2.京剧须生泰斗《马连良唱腔选》第一集
            NormalizeMp3FileName(sheetModels,   // 缺少 "马连良"
                "02.武家坡2 王玉蓉 百代",
                "02.武家坡2 马连良 王玉蓉 百代");
            NormalizeMp3FileName(sheetModels,
                "03.武家坡3 王玉蓉 百代",
                "03.武家坡3 马连良 王玉蓉 百代");
            NormalizeMp3FileName(sheetModels,
                "17.珠帘寨（沉静思）马连良 高亭",
                "17.珠帘寨（沉静思） 马连良 高亭");

            // 12.京剧一枝独秀《管绍华唱腔选》第一集
            NormalizeMp3FileName(sheetModels,
                "04.四郎探母5(坐宫) 王玉蓉 百代",
                "04.四郎探母9(坐宫) 管绍华 王玉蓉 百代");
            NormalizeMp3FileName(sheetModels,
                "05.四郎探母5(坐宫) 王玉蓉 百代",
                "05.四郎探母5(坐宫) 管绍华 王玉蓉 百代");
            NormalizeMp3FileName(sheetModels,
                "09. 四郎探母9(坐宫) 管绍华 王玉蓉 百代",
                "09.四郎探母9(坐宫) 管绍华 王玉蓉 百代");

            // 23.京剧《著名小生专辑》
            NormalizeMp3FileName(sheetModels,
                "19.得意缘3 （说破）毛世来 江世玉 丽歌",
                "19.得意缘3（说破） 毛世来 江世玉 丽歌");
            NormalizeMp3FileName(sheetModels,
                "20.得意缘4 （说破）毛世来 江世玉 丽歌",
                "20.得意缘4（说破） 毛世来 江世玉 丽歌");

            // 2.老北京曲艺大观《鼓界大王刘宝全》 第二集           
            NormalizeMp3FileName(sheetModels,
                "09 博望坡1 刘宝全蓓开",
                "09 博望坡1 刘宝全 蓓开");

            NormalizeMp3FileName(sheetModels,
                "1-05.大保国-1言菊朋 百代",
                "1-05.大保国-1 言菊朋 百代");
            NormalizeMp3FileName(sheetModels,
                "1-07.卖马-1言菊朋、焦宝奎 百代",
                "1-07.卖马-1 言菊朋 焦宝奎 百代");
            NormalizeMp3FileName(sheetModels,
                "1-08.卖马-2言菊朋、焦宝奎 百代",
                "1-08.卖马-2 言菊朋 焦宝奎 百代");
            // NormalizeMp3FileName(sheetModels, 
            //     "1-15.全部让徐州-1 言菊朋、马连昆 百代",
            //     "1-15.全部让徐州-1 言菊朋 马连昆 百代");
            // NormalizeMp3FileName(sheetModels, 
            //     "1-16.全部让徐州-2 言菊朋、马连昆 百代",
            //     "1-16.全部让徐州-2 言菊朋 马连昆 百代");
            NormalizeMp3FileName(sheetModels,
                "1-17.黄鹤楼 言菊朋.吴彦衡.王又荃 百代",
                "1-17.黄鹤楼 言菊朋 吴彦衡 王又荃 百代");
            // NormalizeMp3FileName(sheetModels, 
            //     "1-18.阳平关 言菊朋、吴彦衡 百代",
            //     "1-18.阳平关 言菊朋 吴彦衡 百代");
            NormalizeMp3FileName(sheetModels,   // 格式统一
                "2-05 战北原-1 言菊朋 蓓开",
                "2-05.战北原-1 言菊朋 蓓开");
            NormalizeMp3FileName(sheetModels,
                "2-06 战北原-2 言菊朋 蓓开",
                "2-06.战北原-2 言菊朋 蓓开");

            NormalizeMp3FileName(sheetModels,
                "3-01.捉放宿店-1 胜利",
                "3-01.捉放宿店-1 言菊朋 胜利");
            NormalizeMp3FileName(sheetModels,
                "3-02.捉放宿店-2 胜利",
                "3-02.捉放宿店-2 言菊朋 胜利");
            // NormalizeMp3FileName(sheetModels, 
            //     "3-05.打渔杀家-1 言菊朋、云艳霞 胜利",
            //     "3-05.打渔杀家-1 言菊朋 云艳霞 胜利");
            // NormalizeMp3FileName(sheetModels, 
            //     "3-06.打渔杀家-2 言菊朋、云艳霞 胜利",
            //     "3-06.打渔杀家-2 言菊朋 云艳霞 胜利");
            // NormalizeMp3FileName(sheetModels, 
            //     "3-07.梅龙镇-1 言菊朋、云艳霞 胜利",
            //     "3-07.梅龙镇-1 言菊朋 云艳霞 胜利");
            // NormalizeMp3FileName(sheetModels, 
            //     "3-08.梅龙镇-2 言菊朋、云艳霞 胜利",
            //     "3-08.梅龙镇-2 言菊朋 云艳霞 胜利");

            // 4. 《周信芳唱腔选》
            NormalizeMp3FileName(sheetModels,
                "01.徐策跑城-1周信芳 高亭",
                "01.徐策跑城-1 周信芳 高亭");
            // NormalizeMp3FileName(sheetModels, 
            //     "09.投军别窑-1 周信芳、潘雪艳 蓓开",
            //     "09.投军别窑-1 周信芳 潘雪艳 蓓开");
            // NormalizeMp3FileName(sheetModels, 
            //     "10.投军别窑-2 周信芳、潘雪艳 蓓开",
            //     "10.投军别窑-2 周信芳 潘雪艳 蓓开");
            // NormalizeMp3FileName(sheetModels, 
            //     "15.九更天-1 周信芳、贯盛习 蓓开",
            //     "15.九更天-1 周信芳 贯盛习 蓓开");

            // 5. 《徐碧云唱腔选》
            // NormalizeMp3FileName(sheetModels, 
            //     "04.骊珠梦-2 徐碧云、言菊朋 蓓开",
            //     "04.骊珠梦-2 徐碧云 言菊朋 蓓开");
            // NormalizeMp3FileName(sheetModels, 
            //     "11.女起解-1 徐碧云、萧长华 高亭",
            //     "11.女起解-1 徐碧云 萧长华 高亭");

            // 6. 《郝寿臣唱腔选》
            NormalizeMp3FileName(sheetModels,
                "05.除三害（打窑）-1郝寿臣 蓓开",
                "05.除三害（打窑）-1 郝寿臣 蓓开");
            NormalizeMp3FileName(sheetModels,
                "06.除三害（打窑）-2郝寿臣 蓓开",
                "06.除三害（打窑）-2 郝寿臣 蓓开");
            NormalizeMp3FileName(sheetModels,
                "08.鸿门宴-2郝寿臣 胜利",
                "08.鸿门宴-2 郝寿臣 胜利");
            // NormalizeMp3FileName(sheetModels, 
            //     "13.白良关 郝寿臣、裘桂仙 百代",
            //     "");
            // NormalizeMp3FileName(sheetModels, 
            //     "14.洪羊洞 郝寿臣、裘桂仙 百代",
            //     "");
            // NormalizeMp3FileName(sheetModels, 
            //     "15.普球山-1 郝寿臣、萧长华 百代",
            //     "");
            // NormalizeMp3FileName(sheetModels, 
            //     "16.普球山-2 郝寿臣、萧长华 百代",
            //     "");
            // NormalizeMp3FileName(sheetModels, 
            //     "17.飞虎梦-1 郝寿臣、吴富琴 百代",
            //     "");
            // NormalizeMp3FileName(sheetModels, 
            //     "18.飞虎梦-2 郝寿臣、吴富琴 百代",
            //     "");
            // NormalizeMp3FileName(sheetModels, 
            //     "19.荆轲传-1 郝寿臣、甄洪奎 百代",
            //     "");
            // NormalizeMp3FileName(sheetModels, 
            //     "20.荆轲传-2 郝寿臣、甄洪奎 百代",
            //     "");

            // 7. 《裘桂仙唱腔选》
            // NormalizeMp3FileName(sheetModels, 
            //     "02.黄金台-1 时慧宝、裘桂仙 百代",
            //     "");
            // NormalizeMp3FileName(sheetModels, 
            //     "03.黄金台-2 时慧宝、裘桂仙 百代",
            //     "");
            // NormalizeMp3FileName(sheetModels, 
            //     "12.骂曹 言菊朋、裘桂仙 开明",
            //     "");
            // NormalizeMp3FileName(sheetModels, 
            //     "13.二进宫 言菊朋、裘桂仙 开明",
            //     "");

            // 8. 《侯喜瑞、裘盛戎唱腔选》
            // NormalizeMp3FileName(sheetModels, 
            //     "15.闹江州-1 金少山、裘盛戎 百代",
            //     "");
            // NormalizeMp3FileName(sheetModels, 
            //     "16.闹江州-2 金少山、裘盛戎 百代",
            //     "");
            // NormalizeMp3FileName(sheetModels, 
            //     "17.连环套-1 马德成、裘盛戎 国乐",
            //     "");
            // NormalizeMp3FileName(sheetModels, 
            //     "18.连环套-2 马德成、裘盛戎 国乐",
            //     "");

            // 9. 《萧长华唱腔选》
            // NormalizeMp3FileName(sheetModels, 
            //     "03.八十八扯-1 马富禄、萧长华 胜利",
            //     "03.八十八扯-1 马富禄、萧长华 胜利");
            // NormalizeMp3FileName(sheetModels, 
            //     "04.八十八扯-2 马富禄、萧长华 胜利",
            //     "04.八十八扯-2 马富禄、萧长华 胜利");
            // NormalizeMp3FileName(sheetModels, 
            //     "05.八十八扯-3 马富禄、萧长华 胜利",
            //     "05.八十八扯-3 马富禄、萧长华 胜利");
            // NormalizeMp3FileName(sheetModels, 
            //     "06.八十八扯-4 马富禄、萧长华 胜利",
            //     "06.八十八扯-4 马富禄、萧长华 胜利");
            // NormalizeMp3FileName(sheetModels, 
            //     "07.打侄上坟-1 姜妙香、萧长华 胜利",
            //     "");
            // NormalizeMp3FileName(sheetModels, 
            //     "08.打侄上坟-2 姜妙香、萧长华 胜利",
            //     "08.打侄上坟-2 姜妙香、萧长华 胜利");
            // NormalizeMp3FileName(sheetModels, 
            //     "09.群英会-1 姜妙香、萧长华 胜利",
            //     "09.群英会-1 姜妙香、萧长华 胜利");
            // NormalizeMp3FileName(sheetModels, 
            //     "10.群英会-2 姜妙香、萧长华 胜利",
            //     "10.群英会-2 姜妙香、萧长华 胜利");
            // NormalizeMp3FileName(sheetModels, 
            //     "19.盘关-1 萧长华、萧盛萱 百代",
            //     "19.盘关-1 萧长华、萧盛萱 百代");
            // NormalizeMp3FileName(sheetModels, 
            //     "20.盘关-2 萧长华、萧盛萱 百代",
            //     "20.盘关-2 萧长华 萧盛萱 百代");
            NormalizeMp3FileName(sheetModels,
                "21.打杠子-1萧长华 太平",
                "21.打杠子-1 萧长华 太平");

            // 10. 《汪派老生唱腔选》    
            NormalizeMp3FileName(sheetModels,
                "01.文昭关-1王凤卿 蓓开",
                "01.文昭关-1 王凤卿 蓓开");
            NormalizeMp3FileName(sheetModels,
                "02.文昭关-2王凤卿 蓓开",
                "02.文昭关-2 王凤卿 蓓开");
            NormalizeMp3FileName(sheetModels,
                "03.朱砂痣-1王凤卿 蓓开",
                "03.朱砂痣-1 王凤卿 蓓开");
            NormalizeMp3FileName(sheetModels,
                "04.朱砂痣-2王凤卿 蓓开",
                "04.朱砂痣-2 王凤卿 蓓开");
            NormalizeMp3FileName(sheetModels,
                "13.红拂传-1 郭仲衡、杜丽云 长城",
                "13.红拂传-1 郭仲衡 杜丽云 长城");
            NormalizeMp3FileName(sheetModels,
                "15.举鼎观画-1郭仲衡 大中华",
                "15.举鼎观画-1 郭仲衡 大中华");
            NormalizeMp3FileName(sheetModels,
                "16.举鼎观画-2郭仲衡 大中华",
                "16.举鼎观画-2 郭仲衡 大中华");

            // 梨园名票唱腔选
            NormalizeMp3FileName(sheetModels,
                "01.汾河湾-1顾珏荪、张吾翼 百乐",
                "01.汾河湾-1 顾珏荪、张吾翼 百乐");

            // 2019年出品-曲艺
            NormalizeMp3FileName(sheetModels,
                "4-06.北派坠子《吕蒙正赶斋》2 乔清秀乔利元 胜利",
                "4-06.北派坠子《吕蒙正赶斋》2 乔清秀 乔利元 胜利");
            NormalizeMp3FileName(sheetModels,
                "4-07.北派坠子《马前泼水》1 乔清秀乔利元 胜利",
                "4-07.北派坠子《马前泼水》1 乔清秀 乔利元 胜利");
            NormalizeMp3FileName(sheetModels,
                "4-08.北派坠子《马前泼水》2 乔清秀乔利元 胜利",
                "4-08.北派坠子《马前泼水》2 乔清秀 乔利元 胜利");

            #endregion

            var result = sheetModels.ToArray();

            // 导入没有MP3/MP4地址的记录
            var errorFileName = System.IO.Path.Combine(StaticVariables.GetDownloadDir(), "errors.txt");
            var errorCount = ExportHelper.ExportError(result, errorFileName);
            System.Console.WriteLine($"共找出{errorCount}个错误, log: {errorFileName}");
          
            // // 导出excel总数据
            //ExportHelper.ExportExcel("data.xlsx", result);

            // // 导出歌词
            ExportHelper.ExportLyricToWord(result);
            // 下载封面图片，MP3
            //ExportHelper.DownadMp3AndMp4(result);
            
            //return;

            // 生成MP3标签
            //ExportHelper.GenerateTagInfo(result);
        }

        /// <summary>
        /// 有的MP3文件标题有错, 解析artists会出错
        /// 手动修改MP3Info.Title属性, 和已经下载好的MP3文件名
        /// 
        /// 如
        /// "09. 四郎探母9(坐宫) 管绍华 王玉蓉 百代"
        /// 修改为
        /// "09.四郎探母9(坐宫) 管绍华 王玉蓉 百代"
        /// 否则原标题分组是 "09." "四郎探母9(坐宫)" "管绍华" "王玉蓉" "百代",
        /// 会把"四郎探母9(坐宫)"也当作artist
        /// </summary>
        private static void NormalizeMp3FileName(List<ExcelSheetModel> sheetModels, string oldFileName, string newFileName)
        {
            string categoryName = null, albumName = null;
            Mp3Info wrongMp3 = null;
            // 大分类： 如 2018年出品-京剧
            foreach (var categoryInfo in sheetModels)
            {
                // 该分类下面的专辑: 1.京剧正宗谭派《王又宸专辑》
                foreach (MediaItem mediaItem in categoryInfo.MediaItems)
                {
                    wrongMp3 = mediaItem.Mp3Items?.FirstOrDefault(x => x.Title == oldFileName);
                    if (wrongMp3 != null)
                    {
                        categoryName = categoryInfo.Name;
                        albumName = mediaItem.Title;
                        break;
                    }
                }

                if (wrongMp3 != null)
                {
                    break;
                }
            }

            if (wrongMp3 != null
            && !string.IsNullOrEmpty(categoryName)
            && !string.IsNullOrEmpty(albumName))
            {
                var mp3FilePath = System.IO.Path.Combine(StaticVariables.GetDownloadDir(),
                    categoryName, // "2018年出品-京剧",
                    albumName, // "11 京剧红生大王《林树森专辑》",
                    oldFileName + ".mp3");

                var newMp3FilePath = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(mp3FilePath),
                    newFileName + ".mp3"
                );
                if (System.IO.File.Exists(mp3FilePath))
                {
                    if (System.IO.File.Exists(newMp3FilePath))
                        System.IO.File.Delete(newMp3FilePath);
               
                        System.IO.File.Move(mp3FilePath, newMp3FilePath);       
                }

                // 更新MP3标题
                wrongMp3.Title = newFileName;
            }
        }

        ///<summary>
        /// 返回当前分类下每个专辑的名称, url
        /// http://www.bavc.com.cn/c44169.htm
        ///</summary>
        static List<MediaItem> ParseMediaItems(string url)
        {
            var homePageNode = HtmlCacheParser.LoadHtmlNode(url);

            // 内容页 /html/body/table[4]/tbody/tr/td[3]/p[3]/table[1]
            // /html/body/table[4]
            var mainNode = homePageNode.SelectSingleNode("/html/body/table[4]/tbody/tr/td[3]/table[3]");

            var mediaItemDic = new Dictionary<string, MediaItem>();
            foreach (var link in mainNode.SelectNodes(".//p/a"))
            {
                var navPath = link.Attributes["href"].Value;
                var title = link.InnerText;
                // http://www.bavc.com.cn/xxxx.htm
                var address = StaticVariables.HOST_NAME.TrimEnd('/') + navPath;

                if (mediaItemDic.ContainsKey(title))
                {
                    //HtmlParseLogger.Error($"已经下载过{title}: {address}");
                    //System.Console.WriteLine($"已经下载过{title}: {address}");
                    continue;
                }
                try
                {
                    MediaItem item = ParseMediaItem(title, address);

                    mediaItemDic.Add(title, item);
                }
                catch (Exception ex)
                {
                    DebugInfo(ex.Message);
                }
            }

            return mediaItemDic.Values.ToList();
        }

        // 下载某个具体专辑信息
        // 如：1.《评剧皇后白玉霜 》第一集 http://www.bavc.com.cn/c44169.htm
        static MediaItem ParseMediaItem(string title, string url)
        {
            MediaItem item = new MediaItem();
            item.Title = title;
            item.Url = url;

            DebugInfo($"读取专辑: {item.Title}");
            var homePageNode = HtmlCacheParser.LoadHtmlNode(url);

            // 主要内容的node
            var mainNode = homePageNode.SelectSingleNode("/html/body/table[7]");

            //发布时间: /html/body/table[7]/tbody/tr[1]/td/p[2]/font/span
            item.PublishTime = mainNode.SelectSingleNode(".//font/span")?.InnerText;

            //简介 /html/body/table[7]/tbody/tr[3]/td/div/p[1]
            item.Description = mainNode.SelectSingleNode(".//div[@class='article']")?.InnerText;

            // 封面图片 /html/body/table[7]/tbody/tr[3]/td/div/p[2]/img
            var imageAttr = mainNode.SelectSingleNode(".//div[@class='article']//img")?.Attributes["src"]?.Value;
            if (!string.IsNullOrEmpty(imageAttr))
            {
                item.ImageUrl = StaticVariables.HOST_NAME.TrimEnd('/') + imageAttr;
            }

            // MP3列表 /html/body/table[7]/tbody/tr[7]/td/p/table/tbody/tr[1]/td[2]/a
            foreach (var link in mainNode.SelectNodes(".//a"))
            {
                var href = link.Attributes["href"].Value;
                var mp3Title = link.InnerText;
                // http://www.bavc.com.cn/xxxx.htm
                var address = StaticVariables.HOST_NAME.TrimEnd('/') + href;

                var mp3Info = new Mp3Info
                {
                    Title = mp3Title,
                    Url = address
                };

                // DebugInfo($"\t读取专辑下曲目: {mp3Title}");
                LoadMp3Info(mp3Info);

                item.Mp3Items.Add(mp3Info);
            }

            return item;
        }

        /// url为某个具体MP3播放页面
        /// 从url中读取MP3相关信息
        static void LoadMp3Info(Mp3Info mp3Info)
        {
            if (mp3Info == null || string.IsNullOrEmpty(mp3Info.Url))
                throw new ArgumentException(nameof(mp3Info));

            // 播放页面左侧有导航栏的情况（少数是这样）
            // 无导航： http://www.bavc.com.cn/w10276740.htm?page=1
            // 有导航： http://www.bavc.com.cn/w10276738.htm?page=1
            var homePageNode = HtmlCacheParser.LoadHtmlNode(mp3Info.Url);
            var tdNodes = homePageNode.SelectNodes("/html/body/table[4]/tbody/tr/td");
            if (tdNodes == null || tdNodes.Count == 0)
                HtmlParseLogger.Error($"{mp3Info.Title}({mp3Info.Url})页面信息不存在");

            // 音频页面有导航栏xpath不同
            bool hasSidebar = tdNodes.Count > 1;

            HtmlNode mainNode = null;
            if (!hasSidebar)
                mainNode = homePageNode.SelectSingleNode("/html/body/table[4]/tbody/tr/td/table[4]");
            else
                mainNode = homePageNode.SelectSingleNode("/html/body/table[4]/tbody/tr/td[3]/center/table[last()]");

            if (mainNode == null)
                HtmlParseLogger.Error($"{mp3Info.Title}({mp3Info.Url})页面信息不存在");

            // 标题(列表页已经获取了) /html/body/table[4]/tbody/tr/td/table[4]/tbody/tr[2]/td/table/tbody/tr[1]/td/p[1]/span

            // 当前读取方式会显示‘Your browser does not support’，<a>写出了地址
            mp3Info.Mp3DownloadUrl = mainNode.SelectSingleNode(".//table[1]//table[1]//a")?.Attributes["href"]?.Value;

            // 如果没有MP3, 检测MP4
            if (string.IsNullOrEmpty(mp3Info.Mp3DownloadUrl))
                mp3Info.Mp4DownloadUrl = GuessMp4DownloadUrl(mp3Info.Title, mp3Info.Url);

            // 歌词标题和歌词在一个td里, 标题带有<strong>
            // /html/body/table[4]/tbody/tr/td/table[4]/tbody/tr[2]/td/table/tbody/tr[5]/td/strong
            // /html/body/table[4]/tbody/tr/td/table[4]/tbody/tr[2]/td/table/tbody/tr[4]/td
            // 歌词部分有两种可能
            HtmlNode lyricNode = mainNode.SelectSingleNode("./tbody/tr[last()-1]/td/table/tbody/tr[last()]");
            if (lyricNode == null)
                lyricNode = mainNode.SelectSingleNode("./tbody/tr[last()-2]/td/table/tbody/tr[last()]");

            if (lyricNode == null)
                HtmlParseLogger.Error($"{mp3Info.Title}({mp3Info.Url})歌词信息不存在, 跳过");

            // 大部分标题在<p><strong>{Title}</strong></p>中
            // 目前只发现13. 《梨园名票唱腔选》第三集 "06.武家坡-2 夏山楼主 高亭"没有<strong>, 标题<p>{Title}</p>中
            var lyricTitle = (lyricNode?.SelectSingleNode(".//strong") ??
                                lyricNode?.SelectSingleNode(".//p"))?.InnerText;
            var lyric = lyricNode?.InnerText;

            // .Replace("&nbsp;", "")
            lyric = lyric?.Replace("\r\n", "");

            // 歌词去掉标题部分
            if (!string.IsNullOrEmpty(lyricTitle)
                && !string.IsNullOrEmpty(lyric))
            {
                var titleIndex = lyric.IndexOf(lyricTitle);
                // 标题出现在开头
                if (titleIndex == 0 && lyric.Length > lyricTitle.Length)
                    lyric = lyric.Substring(lyricTitle.Length);
            }

            mp3Info.Lyric = lyric;
            mp3Info.LyricTitle = lyricTitle;
        }

        /// <summary>
        /// 部分页面没有MP3, 只有MP4视频, 需要检测MP4实际地址 (如： 乌盆记-1 言菊朋 http://www.bavc.com.cn/w10279097.htm?page=1)
        /// 找到播放器id部分, 根据id推测出下载地址
        /// player id: <div id="piv_d69fff2eae3766ef7aff664b0ab2b61d_d"></div>
        /// 下载地址模板: http://mpv.videocc.net/d69fff2eae/{0}/{1}_1.mp4
        /// </summary>
        /// 
        /// 以 piv_d69fff2eae3766ef7aff664b0ab2b61d_d 为例
        /// id部分只要两个_之间的字符 d69fff2eae3766ef7aff664b0ab2b61d
        /// mp4地址模板中:
        /// {0} 为 "d69fff2eae3766ef7aff664b0ab2b61d" 最后一个字母d
        /// {1} 为 ”d69fff2eae3766ef7aff664b0ab2b61d“
        /// 则实际MP4地址为: https://mpv.videocc.net/d69fff2eae/d/d69fff2eae3766ef7aff664b0ab2b61d_1.mp4
        public static string GuessMp4DownloadUrl(string title, string mp3InfoUrl)
        {
            // 复制Program.LoadMp3Info
            var homePageNode = HtmlCacheParser.LoadHtmlNode(mp3InfoUrl);

            // <div id="plv_d69fff2eae176f68eb79b5e0575cc75b_d"></div>
            var main2 = homePageNode.SelectSingleNode("/html/body/table[4]/tbody/tr");
            var targetNode = main2.SelectNodes(".//div").FirstOrDefault(x => x.Attributes["id"] != null && x.Attributes["id"].Value.StartsWith("plv_"));

            if (targetNode == null)
                HtmlParseLogger.Error($"mp4 解析错误 {title}({mp3InfoUrl})页面信息不存在");

            var mp4UrlTemplate = "https://mpv.videocc.net/d69fff2eae/{0}/{1}_1.mp4";

            var idVal = targetNode.Attributes["id"].Value;
            var first_ = idVal.IndexOf("_");
            var last_ = idVal.LastIndexOf("_");

            if (first_ > -1 && last_ > -1 && first_ < last_)
            {
                //"plv_d69fff2eae176f68eb79b5e0575cc75b_d"
                // d69fff2eae176f68eb79b5e0575cc75b
                var mp4ItemKey = idVal.Substring(first_ + 1, last_ - first_ - 1);

                return string.Format(mp4UrlTemplate, mp4ItemKey[mp4ItemKey.Length - 1], mp4ItemKey);
            }

            return string.Empty;
        }

        static void DebugInfo(string info, bool newline = true)
        {
            if (newline)
                Console.WriteLine(info);
            else
                Console.Write(info);
        }

        /// 测试导出excel代码
        private void TestExcelExport()
        {

            var homePageNode = HtmlCacheParser.LoadHtmlNode("http://www.bavc.com.cn/c44167.htm");

            // 内容页 /html/body/table[4]/tbody/tr/td[3]/p[3]/table[1]
            var mainContent = homePageNode.SelectSingleNode("/html/body/table[4]/tbody/tr/td[3]/table[3]");

            List<string> urls = new List<string>();
            foreach (var link in mainContent.SelectNodes(".//a"))
            {
                var navPath = link.Attributes["href"].Value;
                var title = link.InnerText;

                var address = "http://www.bavc.com.cn" + navPath;

                urls.Add(address);

                System.Console.WriteLine($"{title}, {address}");
            }

            ExcelSheetModel model = new ExcelSheetModel
            {
                Name = "test",
                MediaItems = urls.Select(x => new MediaItem
                {
                    Title = x
                }).ToList()
            };

            ExcelHelper.Save("test.xlsx", new ExcelSheetModel[] { model });
            return;
        }
    }
}
