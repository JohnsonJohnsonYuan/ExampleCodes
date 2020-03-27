
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;

namespace OperaHtmlParser
{
    public static class ExportHelper
    {
        // 1. 读取分类, 专辑, 专辑下音频地址, 导出到excel
        public static void ExportExcel(string filePath, ExcelSheetModel[] sheetModels)
        {
            Console.WriteLine($"导出所有数据到{filePath}...");
            ExcelHelper.Save(filePath, sheetModels);
            Console.WriteLine($"导出所有数据到{filePath}完成");
        }

        // 导出唱词
        public static void ExportLyricToWord(ExcelSheetModel[] sheetModels)
        {
            Console.WriteLine($"导出所有歌词...");
            WordHelper.Save(sheetModels);
            Console.WriteLine($"导出所有歌词完成");
        }

        // 下载MP3, mp4
        public static void DownadMp3AndMp4(ExcelSheetModel[] sheetModels)
        {
            new DownloadHelper().Save(sheetModels);
        }

        /// <summary>
        /// 导出没有下载地址的MP3(部分只有mp4视频，没有提供音频，手动下载)
        /// </summary>
        /// <param name="sheetModels"></param>
        /// <returns>error count</returns>
        public static int ExportError(ExcelSheetModel[] sheetModels, string errorLogPath)
        {
            if (File.Exists(errorLogPath))
                File.Delete(errorLogPath);

            var totalErrorCount = 0;
            using (var writer = new StreamWriter(errorLogPath))
            {
                foreach (var category in sheetModels)
                {
                    foreach (MediaItem mediaItem in category.MediaItems)
                    {
                        Console.WriteLine($"检查专辑{mediaItem.Title}");

                        var errors = new List<string>();

                        foreach (var mp3Info in mediaItem.Mp3Items)
                        {
                            if (string.IsNullOrEmpty(mp3Info.Mp3DownloadUrl)
                                && string.IsNullOrEmpty(mp3Info.Mp4DownloadUrl))
                            {
                                // MP3没有地址 则是MP4视频
                                errors.Add($"{mp3Info.Title}\t{mp3Info.Url} 没有mp3/mp4下载地址");
                            }

                            if (mp3Info.GuessArtists(mediaItem.Title, out _).Length == 0)
                            {
                                errors.Add($"{mp3Info.Title}没有Artist, 查看标题格式是否错误");
                            }
                        }

                        if (errors.Count > 0)
                        {
                            totalErrorCount += errors.Count;
                            writer.WriteLine($"{mediaItem.Title} ({mediaItem.Url})");

                            writer.WriteLine(string.Join(Environment.NewLine, errors));
                        }
                    }
                }
            }
            return totalErrorCount;
        }

        public static void GenerateTagInfo(ExcelSheetModel[] sheetModels)
        {
            System.Console.WriteLine("生成mp3标签..");
            TagHelper.GenerateTagInfo(sheetModels);
            System.Console.WriteLine("生成标签完成");
        }
    }

    /// 写入MP3tag标签
    /// 默认读取"_avatar.jpg"作为封面, 如果没有则不设置
    public class TagHelper
    {
        public static void GenerateTagInfo(ExcelSheetModel[] sheetModels)
        {
            if (sheetModels == null)
                return;

            #region 默认专辑封面名称为_avatar.jpg, 需要手动裁剪, 先根据下载来的图片_covert.jpg复制一个,编辑之后直接覆盖

            // 找出所有_cover.jpg
            var albumCovers = Directory.GetFiles(StaticVariables.GetDownloadDir(), StaticVariables.ALBUM_COVER_NAME, SearchOption.AllDirectories);
            foreach (var albumCoverFile in albumCovers)
            {
                var coverDir = Path.GetDirectoryName(albumCoverFile);

                // _avatar.jpg
                var mp3CoverFile = Path.Combine(coverDir, StaticVariables.ALBUM_MP3_COVER_NAME);

                // 如果存在则跳过
                if (File.Exists(mp3CoverFile))
                    continue;

                File.Copy(albumCoverFile, mp3CoverFile, true);
                System.Console.WriteLine("$复制专辑{Path.GetDirectoryName(coverDir)}封面");
            }

            #endregion

            // 大分类： 如 2018年出品-京剧
            foreach (var categoryInfo in sheetModels)
            {
                // 该分类下面的专辑: 1.京剧正宗谭派《王又宸专辑》
                foreach (MediaItem mediaItem in categoryInfo.MediaItems)
                {
                    System.Console.WriteLine($"专辑{mediaItem.Title}生成标签...");

                    foreach (var mp3Info in mediaItem.Mp3Items)
                    {
                        // 当前专辑文件夹
                        var mp3FileDir = Path.Combine(
                            StaticVariables.GetDownloadDir(),   // _downloads
                            categoryInfo.Name,                  // 2018年出品-京剧
                            mediaItem.Title                     // 1.京剧正宗谭派《王又宸专辑》
                        );

                        // mp3 地址
                        var mp3FilePath = Path.Combine(
                            mp3FileDir,
                            mp3Info.Title + ".mp3"              // 01.李陵碑1 王又宸 高亭.mp3
                        );

                        if (!System.IO.File.Exists(mp3FilePath))
                        {
                            HtmlParseLogger.TagError($"{mp3Info.Title}.mp3文件不存在, 跳过生成tag");
                            continue;
                        }
                        try
                        {
                            using (var fs = new FileStream(mp3FilePath, FileMode.Open))
                            {
                                var tagFile = TagLib.File.Create(
                                   new TagLib.StreamFileAbstraction(mp3FilePath, fs, fs));

                                var tags = tagFile.GetTag(TagLib.TagTypes.Id3v2);

                                // 设置标签属性
                                SetTags(tags, mp3FileDir, mp3Info, mediaItem);

                                tagFile.Save();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            HtmlParseLogger.TagError($"生成{mp3Info.Title}.mp3 出错, {ex.Message}");
                            continue;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 生成标签
        /// </summary>
        /// <param name="tags">MP3标签</param>
        /// <param name="mp3FileDir">MP3文件夹</param>
        /// <param name="mp3Info">MP3信息</param>
        /// <param name="mediaItem">MP3所在专辑信息</param>
        private static void SetTags(TagLib.Tag tag, string mp3FileDir, Mp3Info mp3Info, MediaItem mediaItem)
        {
            tag.Title = mp3Info.Title;
            tag.Album = mediaItem.Title;
            var artists = mp3Info.GuessArtists(mediaItem.Title, out string companyName);
            if (artists != null && artists.Length > 0)
            {
                tag.AlbumArtists = new string[1] { artists[0] };
                tag.Artists = artists;
            }

            // 唱片公司写到Grouping属性中?
            if (string.IsNullOrEmpty(companyName))
                tag.Grouping = companyName;

            var trackIndex = mediaItem.Mp3Items?.FindIndex(x => x.Title == mp3Info.Title);
            if (trackIndex > -1)
            {
                tag.Track = (uint)trackIndex + 1;
                tag.TrackCount = (uint)mediaItem.Mp3Items.Count;
            }

            // 播放器不支持内嵌歌词, 目前还是放到备注里了
            //tag.Lyrics = mp3Info.Lyric;
            tag.Comment = mp3Info.Lyric; //tag.Comment = "强森强森整理";

            tag.Copyright = "北京典籍与经典老唱片数字化出版项目";

            // 封面图片(默认名称为_avatar.jpg)
            var mp3CoverFilePath = Path.Combine(mp3FileDir, StaticVariables.ALBUM_MP3_COVER_NAME);

            if (System.IO.File.Exists(mp3CoverFilePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(mp3CoverFilePath);
                var corverPic = new TagLib.Picture(new TagLib.ByteVector(fileBytes));
                // apply corver
                tag.Pictures = new TagLib.IPicture[] { corverPic };
            }
        }
    }

    /// 下载文件, mp3
    public class DownloadHelper
    {
        // 保存MP3
        public void Save(ExcelSheetModel[] sheetItems)
        {
            foreach (var item in sheetItems)
            {
                // 每个年份分类下一个文件夹, 每个年份下多个专辑
                // 每个专辑一个文件夹， 下面有唱词，MP3
                foreach (var mediaItem in item.MediaItems)
                {
                    // 年份 + 专辑名称

                    // MP3下载文件夹
                    var mp3DownloadDir = Path.Combine(
                        StaticVariables.GetDownloadDir(),
                        item.Name,
                        mediaItem.Title);
                    // MP4 文件单独放一个文件夹, 以后转换为MP3方便
                    var mp4DownloadDir = Path.Combine(
                        StaticVariables.GetDownloadDir(),
                        StaticVariables.MP4_FOLDER_NAME,
                        item.Name,
                        mediaItem.Title);

                    DownloadMediaItem(mp3DownloadDir, mp4DownloadDir, mediaItem);
                }
            }
        }

        // 下载专辑
        private void DownloadMediaItem(string mp3DownloadDir, string mp4DownloadDir, MediaItem mediaItem)
        {
            if (!Directory.Exists(mp3DownloadDir))
                Directory.CreateDirectory(mp3DownloadDir);
            if (!Directory.Exists(mp4DownloadDir))
                Directory.CreateDirectory(mp4DownloadDir);

            System.Console.WriteLine($"下载专辑: {mediaItem.Title}...");

            // key: url, value: 文件保存地址
            Dictionary<string, string> downloadItems = new Dictionary<string, string>();

            // 下载封面图片 "_cover.jpg" 放到MP3文件夹中
            var coverImgName = Path.Combine(mp3DownloadDir, StaticVariables.ALBUM_COVER_NAME);
            downloadItems.Add(mediaItem.ImageUrl, coverImgName);

            // 准备mp3/mp4下载地址
            foreach (var mp3Info in mediaItem.Mp3Items)
            {
                var saveFilePath = string.Empty;
                var downloadUrl = string.Empty;
                if (!string.IsNullOrEmpty(mp3Info.Mp3DownloadUrl))
                {
                    downloadUrl = mp3Info.Mp3DownloadUrl;
                    saveFilePath = Path.Combine(mp3DownloadDir, $"{mp3Info.Title}.mp3");
                }
                else if (!string.IsNullOrEmpty(mp3Info.Mp4DownloadUrl))
                {
                    downloadUrl = mp3Info.Mp4DownloadUrl;
                    saveFilePath = Path.Combine(mp4DownloadDir, $"{mp3Info.Title}.mp4");
                }
                else
                {
                    HtmlParseLogger.Error($"{mediaItem.Title} 没有下载地址，跳过");
                    continue;
                }

                downloadItems.Add(downloadUrl, saveFilePath);
            }

            // 开始下载
            var totalCount = mediaItem.Mp3Items?.Count;
            System.Console.WriteLine($"开始下载专辑{mediaItem.Title}, 共{totalCount}个音频");

            DownadFiles(mediaItem.Title, downloadItems);
        }

        /// <summary>
        /// // key: url, value: 保存地址
        /// </summary>
        /// <param name="downloadItems"></param>
        private void DownadFiles(string albumTitle, Dictionary<string, string> downloadItems, int paralleCount = 4)
        {
            var totalCount = downloadItems.Values?.Count;
            if (totalCount == 0)
                return;

            System.Console.WriteLine($"开始下载专辑{albumTitle}, 共{totalCount}个音频");
            Stopwatch timer = Stopwatch.StartNew();

            var options = new ParallelOptions { MaxDegreeOfParallelism = paralleCount };
            Parallel.ForEach(downloadItems, options, downloadItem =>
            {
                var url = downloadItem.Key;
                var savePath = downloadItem.Value;

                try
                {
                    DownloadSingleFile(url, savePath);
                }
                catch (Exception ex)
                {
                    HtmlParseLogger.Error($"下载{Path.GetFileName(savePath)}出错({url}), Exception: {ex.Message}");
                }

                Console.WriteLine($"下载完成, 剩余: {--totalCount}个");
            });
            timer.Stop();
            System.Console.WriteLine($"专辑{albumTitle}下载完成, 用时{timer.Elapsed.TotalSeconds}s, 共{totalCount}个音频");
        }
        private void DownloadSingleFile(string url, string savePath, bool deleteIfExist = false)
        {
            if (File.Exists(savePath))
            {
                if (deleteIfExist)
                {
                    File.Delete(savePath);
                }
                else
                {
                    System.Console.WriteLine($"{savePath}文件已经存在, 跳过");
                    return;
                }
            }

            HtmlParseLogger.Info($"下载: {Path.GetFileNameWithoutExtension(savePath)}到{savePath}({url})...");
            using (var client = new WebClient())
            {
                client.DownloadFile(url, savePath);
            }
        }

        /// <summary>
        /// 下载MP4
        /// </summary>
        /// <param name="mp4Items"></param>
        [System.Obsolete("已经把下载MP3，MP4全部放到Save方法中")]
        public void DownloadMp4Items(List<Mp4Item> mp4Items, bool deleteIfExist = false)
        {
            var mp4DownloadDIr = System.IO.Path.Combine(
                StaticVariables.GetDownloadDir(),
                StaticVariables.MP4_FOLDER_NAME);

            if (Directory.Exists(mp4DownloadDIr))
                Directory.CreateDirectory(mp4DownloadDIr);

            foreach (var categoryInfo in mp4Items.GroupBy(x => x.CategoryName))
            {
                var categoryName = categoryInfo.Key;
                foreach (var albumInfo in categoryInfo.GroupBy(x => x.AblumName))
                {
                    // albumInfo
                    var albumName = albumInfo.Key;
                    var albumMp4Items = albumInfo.ToArray();
                    if (albumMp4Items.Length == 0)
                        continue;

                    // key: url, value: 保存地址
                    Dictionary<string, string> downloadItems = new Dictionary<string, string>();

                    foreach (var item in albumMp4Items)
                    {
                        if (string.IsNullOrEmpty(item.Mp4DownloadUrl))
                        {
                            HtmlParseLogger.Error($"{item.Mp3FileName} 没有下载地址，跳过");
                            continue;
                        }

                        var dir = Path.Combine(mp4DownloadDIr,
                            categoryName,
                            albumName);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);

                        var mp3Name = Path.Combine(dir, $"{item.Mp3FileName}.mp4");
                        downloadItems.Add(item.Mp4DownloadUrl, mp3Name);
                    }

                    DownadFiles(albumName, downloadItems);
                }
                //OperaHtmlParser.Mp4Item
            }
        }
    }

    // 导出到word
    public static class WordHelper
    {
        public static void Save(ExcelSheetModel[] sheetItems)
        {
            #region 读取模板中样式

            // 默认新创建word没有Heading1,Heading2...这些样式，需要手动创建
            // 采用建一个word样式模板方法
            // 参考(第2个答案)：
            // https://stackoverflow.com/questions/2643822/how-can-i-use-predefined-formats-in-docx-with-poi
            /*
            XWPFDocument template = new XWPFDocument(new FileInputStream(new File("Template.dotx")));       

            XWPFDocument doc = new XWPFDocument();      
            // let's copy styles from template to new doc
            XWPFStyles newStyles = doc.createStyles();
            newStyles.setStyles(template.getStyle());   // 复制模板样式到当前文件

            XWPFParagraph para = doc.createParagraph();
            para.setStyle("Heading1");

            XWPFRun run = para.createRun();
            run.setText("Heading 1");

            return doc;
            */

            NPOI.OpenXmlFormats.Wordprocessing.CT_Styles templateStyle = null;

            if (File.Exists("template.docx"))
            {
                using (var stream = new System.IO.FileStream("template.docx", System.IO.FileMode.Open))
                {
                    XWPFDocument doc = new XWPFDocument(stream);
                    templateStyle = doc.GetCTStyle();
                }
            }
            else
            {
                System.Console.WriteLine("模板文件不存在, 生成word样式会有问题!");
            }

            #endregion

            var downloadDir = StaticVariables.GetDownloadDir();

            if (!Directory.Exists(downloadDir))
            {
                Directory.CreateDirectory(downloadDir);
            }

            #region 每个专辑一个歌词文件
            /*
            foreach (var item in sheetItems)
            {
                // 每个专辑一个唱词文件
                foreach (var mediaItem in item.MediaItems)
                {
                    var subDir = Path.Combine(downloadDir, item.Name, "唱词");
                    if (!Directory.Exists(subDir))
                    {
                        Directory.CreateDirectory(subDir);
                    }

                    var fileName = Path.Combine(subDir, mediaItem.Title + ".docx");

                    using (FileStream fs = new FileStream(fileName, FileMode.Create))
                    {
                        var doc = new XWPFDocument();
                        doc.GetStyles().SetStyles(templateStyle);

                        WriteMedia(doc, mediaItem);
                        doc.Write(fs);
                    }
                }
            }
            */
            #endregion

            #region 所有专辑合并成一个文件

            // 全部的歌词
            var allLyricDoc = new XWPFDocument();
            allLyricDoc.GetStyles().SetStyles(templateStyle);

            foreach (var item in sheetItems)
            {
                // TODO : 前面已经创建过文件
                var subDir = Path.Combine(downloadDir, item.Name);

                if (!Directory.Exists(subDir))
                {
                    Directory.CreateDirectory(subDir);
                }

                try
                {
                    // 每个分类(2018年出品-京剧, 2019年出品-京剧)一个歌词文件
                    var fileName = Path.Combine(subDir, "_所有唱词_" + item.Name + ".docx");

                    using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                    {
                        var doc = new XWPFDocument();
                        doc.GetStyles().SetStyles(templateStyle);

                        // 当前分类下歌词
                        WriteToDoc(doc, item);

                        // 全部歌词
                        WriteToDoc(allLyricDoc, item);

                        doc.Write(fs);
                    }
                }
                catch (System.Exception)
                {
                    continue;
                }
            }

            // 保存全部歌词
            var allLyricFilePath = Path.Combine(downloadDir, "所有唱词.docx");
            using (FileStream fs = new FileStream(allLyricFilePath, FileMode.OpenOrCreate))
            {
                allLyricDoc.Write(fs);
            }
            #endregion
        }

        private static void WriteToDoc(XWPFDocument doc, ExcelSheetModel item)
        {
            var p1 = doc.CreateParagraph();
            p1.Style = "Heading1";
            p1.Alignment = ParagraphAlignment.CENTER;
            XWPFRun r1 = p1.CreateRun();
            // r1.FontSize = 16;
            // r1.IsBold = true;
            r1.SetText(item.Name);

            foreach (var media in item.MediaItems)
            {
                WriteMedia(doc, media);
            }
        }

        private static void WriteMedia(XWPFDocument doc, MediaItem media)
        {
            var p2 = doc.CreateParagraph();
            p2.Style = "Heading2";  // 专辑名
            XWPFRun r2 = p2.CreateRun();
            // r2.FontSize = 14;
            // r2.IsBold = true;
            r2.SetText($"{media.Title}");
            // doc.CreateParagraph().CreateRun().SetText(media.Url);
            doc.CreateParagraph().CreateRun().SetText(media.Description);
            doc.CreateParagraph().CreateRun().SetText("");

            foreach (var mp3Item in media.Mp3Items)
            {
                var lyricTitlePara = doc.CreateParagraph();
                lyricTitlePara.Style = "LyricTitle";
                var title = lyricTitlePara.CreateRun();
                title.SetText(mp3Item.Title);
                //title.SetColor("#FA6922");
                //title.FontSize = 12;
                //title.IsBold = true;

                if (!string.IsNullOrEmpty(mp3Item.LyricTitle))
                {
                    // 绿色歌词标题
                    var p4 = doc.CreateParagraph();
                    p4.Style = "LyricInfo";
                    var lyricTitle = p4.CreateRun();
                    lyricTitle.SetText(mp3Item.LyricTitle);
                    //lyricTitle.SetColor("#006400");
                    //lyricTitle.IsBold = true;
                }

                if (!string.IsNullOrEmpty(mp3Item.Lyric))
                {
                    var lyricPara = doc.CreateParagraph();
                    lyricPara.Style = "Lyric";
                    lyricPara.CreateRun().SetText(mp3Item.Lyric);
                }

                // if (!string.IsNullOrEmpty(mp3Item.Url))
                // {
                //     doc.CreateParagraph().CreateRun();
                //     doc.CreateParagraph().CreateRun().SetText($"({mp3Item.Url})");
                // }

                doc.CreateParagraph().CreateRun();
            }
        }
    }

    // 导出到excel
    public static class ExcelHelper
    {
        #region Fields

        // 避免每次 hssfworkbook.CreateCellStyle() 耗时
        private static ICellStyle _cachedStyle;

        #endregion

        #region Utilities
        public static void WriteToCell(ISheet sheet, int rowIndex, int colIndex, string content, short? backColor = null)
        {
            var row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
            var cell = row.GetCell(colIndex) ?? row.CreateCell(colIndex);

            if (backColor.HasValue)
            {
                var defaultStyle = FullBorderStyle(sheet.Workbook);

                defaultStyle.FillPattern = FillPattern.SolidForeground;
                defaultStyle.FillForegroundColor = backColor.Value;
                cell.CellStyle = defaultStyle;
            }

            cell.SetCellValue(content);
        }
        public static ICellStyle FullBorderStyle(IWorkbook hssfworkbook)
        {
            if (_cachedStyle == null)
                _cachedStyle = hssfworkbook.CreateCellStyle();

            ICellStyle style = _cachedStyle;

            style.BorderTop = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            return style;
        }

        #endregion

        public static void Save(string fileName, ExcelSheetModel[] sheetItems)
        {
            fileName = Path.Combine(StaticVariables.GetDownloadDir(), fileName);

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                IWorkbook workbook = new XSSFWorkbook();

                foreach (var item in sheetItems)
                {
                    WriteToSheet(workbook, item);
                }

                workbook.Write(fs);

                _cachedStyle = null;
            }
        }

        private static void WriteToSheet(IWorkbook workbook, ExcelSheetModel sheetModel)
        {
            System.Console.WriteLine(sheetModel.Name);
            ISheet sheet = workbook.CreateSheet(sheetModel.Name);

            // titleRow
            WriteToCell(sheet, 0, 0, "名称", HSSFColor.LightBlue.Index);
            WriteToCell(sheet, 0, 1, "Url");
            WriteToCell(sheet, 0, 2, "发布时间");
            WriteToCell(sheet, 0, 3, "简介");
            WriteToCell(sheet, 0, 4, "封面图片地址");

            WriteToCell(sheet, 0, 5, "音频名称");
            WriteToCell(sheet, 0, 6, "音频信息名称");
            WriteToCell(sheet, 0, 7, "歌词标题", HSSFColor.LightGreen.Index);
            WriteToCell(sheet, 0, 8, "歌词", HSSFColor.LightGreen.Index);
            WriteToCell(sheet, 0, 9, "mp3地址", HSSFColor.LightGreen.Index);
            WriteToCell(sheet, 0, 10, "mp4地址", HSSFColor.LightGreen.Index);
            WriteToCell(sheet, 0, 11, "Artists", HSSFColor.LightGreen.Index);

            int dataRowIndex = 1;
            foreach (var mediaItem in sheetModel.MediaItems)
            {
                System.Console.WriteLine($"\t{mediaItem.Title}");

                WriteToCell(sheet, dataRowIndex, 0, mediaItem.Title);
                WriteToCell(sheet, dataRowIndex, 1, mediaItem.Url);
                WriteToCell(sheet, dataRowIndex, 2, mediaItem.PublishTime);
                WriteToCell(sheet, dataRowIndex, 3, mediaItem.Description);
                WriteToCell(sheet, dataRowIndex, 4, mediaItem.ImageUrl);

                // 每个专辑的音频内容
                var itemCount = 0;
                foreach (var mp3Item in mediaItem.Mp3Items)
                {
                    // 给当前专辑前面的空格画边框
                    if (itemCount++ > 0)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            WriteToCell(sheet, dataRowIndex, i, null);
                        }
                    }

                    WriteToCell(sheet, dataRowIndex, 5, mp3Item.Title);
                    WriteToCell(sheet, dataRowIndex, 6, mp3Item.Url);
                    WriteToCell(sheet, dataRowIndex, 7, mp3Item.LyricTitle);
                    WriteToCell(sheet, dataRowIndex, 8, mp3Item.Lyric);
                    WriteToCell(sheet, dataRowIndex, 9, !string.IsNullOrEmpty(mp3Item.Mp3DownloadUrl) ? mp3Item.Mp3DownloadUrl : "无");
                    WriteToCell(sheet, dataRowIndex, 10, !string.IsNullOrEmpty(mp3Item.Mp4DownloadUrl) ? mp3Item.Mp4DownloadUrl : "无");

                    // MP3艺术家(album artists 标签)
                    var artistIndex = 11;
                    foreach (var artist in mp3Item.GuessArtists(mediaItem.Title, out _))
                    {
                        WriteToCell(sheet, dataRowIndex, artistIndex, artist);
                        ++artistIndex;
                    }

                    // next line
                    ++dataRowIndex;
                }

                // next line
                ++dataRowIndex;
            }

            int[] autoWidthCols = new int[] { 0, 2, 5, 9 };
            foreach (var i in autoWidthCols)
            {
                // sheet.AutoSizeColumn(i);
                sheet.SetColumnWidth(i, 256 * 30);
            }
        }
    }

    public class ExcelSheetModel
    {
        public string Name { get; set; }

        public List<MediaItem> MediaItems { get; set; } = new List<MediaItem>();
    }
}