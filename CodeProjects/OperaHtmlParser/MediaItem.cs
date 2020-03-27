using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OperaHtmlParser
{
    public class MediaItem
    {
        private string _description;

        public string Title { get; set; }
        public string Url { get; set; }
        public string PublishTime { get; set; }
        /// <summary>
        /// 艺术家介绍
        /// </summary>
        /// <value></value>
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value?.Replace("&nbsp;", " ").Replace("  ", " ");
            }
        }
        public string ImageUrl { get; set; }

        public List<Mp3Info> Mp3Items { get; set; } = new List<Mp3Info>();
    }

    // http://www.bavc.com.cn/w10276719.htm?page=1
    public class Mp3Info
    {
        private string _lyric;
        private string _lyricTitle;

        // 01.李陵碑1 王又宸 高亭
        public string Title { get; set; }
        // MP3 介绍地址
        public string Url { get; set; }
        // 《李陵碑》第一段 高亭公司唱片 3074A TEB715 1936年录制 王又宸饰杨继业 
        public string LyricTitle
        {
            get
            {
                return _lyricTitle;
            }
            set
            {
                _lyricTitle = value?.Replace("&nbsp;", " ").Replace("  ", " ");
            }
        }
        //  [反二黄慢板]金沙滩双龙会一仗败了，只杀得血成河鬼哭神嚎。我那大郎儿替宋王[快三眼]把忠尽了，二郎儿短剑下命赴阴曹，杨三郎被马踏尸骨难找，四八郎失番邦无有下梢，五郎儿弃红尘剃发修道，夜得梦七郎儿箭射在芭蕉，只剩下六郎儿去把贼讨，可怜他尽得忠又尽孝，东荡西杀、南征北剿、血战沙场、马不停蹄、为国勤劳，可叹我八个子，把四子丧了，我把四子丧了！我的儿啊！ 
        public string Lyric
        {
            get
            {
                return _lyric?.TrimStart(' ');
            }
            set
            {
                _lyric = value?.Replace("&nbsp;", " ").Replace("  ", " ");
            }
        }

        // 实际url http://bavc.hi300.cn/jingju/wangyouchen/01.mp3
        public string Mp3DownloadUrl { get; set; }
        
        /// <summary>
        /// 部分专辑只有MP4地址
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
        public string Mp4DownloadUrl { get; set; }

        /// <summary>
        /// 猜测当前MP3的艺术家(生成标签时Artists字段)
        /// </summary>
        /// <param name="ablumName">当前专辑名称</param>
        /// <param name="companyName">唱片公司, 百代</param>
        /// <example>
        /// 如专辑名为
        /// 2.京剧须生泰斗《马连良唱腔选》第一集
        /// 
        /// MP3有多个和1个艺术家的情况
        /// 01.武家坡1 马连良 王玉蓉 百代
        /// 02.武家坡2 王玉蓉 百代
        /// 02.鼎盛春秋2 金少山 谭富英 百代
        /// 
        /// 从MP3标题中猜测艺术家, 按照空格分组, 去掉第一个和最后一个, 中间的就是艺术家
        /// 如果出现多个艺术家, 如果专辑名称中有该名称, 则把该艺术家认为主要艺术家
        /// </example>
        /// <returns></returns>
        public string[] GuessArtists(string ablumName, out string companyName)
        {
            companyName = null;
            if (string.IsNullOrEmpty(Title))
                return new string[0];

            string[] splits = Title.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            // 第一个是名称, 最后一个是唱片公司, 中间的是艺术家
            if (splits.Length < 3)
                return new string[0];

            // 唱片公司名称在最后一个
            companyName = splits[splits.Length - 1];

            // 网页信息有的MP3标题可能错误, 如下面得到的artist就包括了错误信息
            // 01 大西厢1 刘宝全 百代 => 大西厢1, 刘宝全, 实际上只需要刘宝全
            // 正确的标题应该是 01大西厢1 刘宝全 百代, 如果出现这种情况跳过第二个
            // 默认artist从第二个开始找
            var artistStartIndex = 1;
            Regex trackNumRegex = new Regex(@"^\d{2,2}\.?$", RegexOptions.Compiled);
            if (trackNumRegex.IsMatch(splits[0]))
            {
                // 如果第一部分只有序号, 则该标题原信息就错误, 从第三个开始才是artist
                artistStartIndex = 2;
            }

            var mainArtist = string.Empty;
            List<string> artists = new List<string>();
            for (int i = artistStartIndex; i < splits.Length - 1; i++)
            {
                var artist = splits[i];
                if (!string.IsNullOrEmpty(ablumName)
                    && ablumName.Contains(artist))
                {
                    // 如果mainArtist已经有值, 手动查看是否有多个艺术家
                    if (!string.IsNullOrEmpty(mainArtist))
                        throw new System.Exception($"{Title}出现多个artist, 需要查看, 专辑{ablumName}");

                    mainArtist = artist;
                }

                if (!artists.Contains(artist))
                    artists.Add(artist);
            }

            // mainArtist是否在第一个
            if (!string.IsNullOrEmpty(mainArtist)
            && artists.Count > 0
            && artists[0] != mainArtist)
            {
                artists.Remove(mainArtist);
                artists.Insert(0, mainArtist);
            }

            // 有时候会出现 "郭仲衡、杜丽云" 这种情况, 需要分成两个
            // 重新给artists赋值
            if (artists.Count == 1)
            {
                var separatorIndex = artists[0].IndexOf("、");
                if (separatorIndex > -1)
                {
                    // 只处理有两个artist的情况, 目前只有两个
                    var splits2 = artists[0].Split('、');
                    if (splits2.Length == 2)
                    {
                        artists.Clear();
                        if (ablumName.Contains(splits2[0]))
                        {
                            artists.Add(splits2[0]);
                            artists.Add(splits2[1]);
                        }
                        else
                        {
                            artists.Add(splits2[1]);
                            artists.Add(splits2[0]);
                        }
                    }
                    else
                    {
                        HtmlParseLogger.TagError($"{this.Title} : {artists[0]}包含超过2个aritst, 手动查看");
                    }
                }
            }

            // 删除最后一个artist的"等"字
            // 如：09.甘露寺1 马连良 马连昆等 高亭 错误结果是: "马连良", "马连昆等"
            if (artists.Count > 1)
            {
                // 将"马连昆等"修改为"马连昆"
                var lastArtist = artists[artists.Count - 1];
                if (lastArtist.Length > 1
                && lastArtist[lastArtist.Length - 1] == '等')
                {
                    artists[artists.Count - 1] = lastArtist.Substring(0, lastArtist.Length - 1);
                }
            }

            return artists.ToArray();
        }
    }

    [System.Obsolete("已经把下载MP3，MP4全部放到Save方法中")]
    public class Mp4Item
    {
        // 2019年出品-京剧
        public string CategoryName { get; set; }
        // 1. 《言菊朋唱腔选》第1集
        public string AblumName { get; set; }
        // 播放页面地址
        public string Url { get; set; }
        // mp4地址
        public string Mp4DownloadUrl { get; set; }
        // MP3文件名
        public string Mp3FileName { get; set; }
    }
}