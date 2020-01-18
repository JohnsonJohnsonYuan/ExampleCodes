# 文件说明（北京典籍与经典老唱片数字化出版项目）

* 使用的库有: NOPI, HtmlAgilityPack, TagLib(mp3标签)

* template.docx: 歌词文件模板, 定义了Header1, Header2..., 默认NOPI word没有Style
* _parseCache.json: 把读取html获取到的专辑信息保存下来, 如果有该文件则直接读取, 否则读取网址, 缓存html, 默认保存在_htmlCache文件夹下
* 导出数据示例文件夹: 所有专辑信息和歌词导出示例