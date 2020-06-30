import requests
from lxml import etree
import os
from queue import Queue
from pathlib import Path
import time
from threading import Thread
import json

"""
下载评书网站http://shantianfang.zgpingshu.com/下某个专辑下所有MP3

用到的包:
    requests: 请求返回html
    lxml: xpath 查找

getDownloadItems方法中配置要下载的播放列表
"""

# 下载单田芳 大明英烈(全180回)全集 为例
hostUrl = "http://shantianfang.zgpingshu.com"
playlistId = 4151
playlistUrl = "http://shantianfang.zgpingshu.com/%d/" % playlistId

def getContent(url):
    try:
        #print("解析" + url)
        r = requests.get(url)
        r.encoding = "gb2312"
        return r.text
    except:
        print(url + '读取错误')

def getETree(url):
    try:
        text = getContent(url)
        return etree.HTML(text)
    except:
        print(url + ' 读取错误')

'''
返回正确的下载地址(完整url)
下载地址<a>的href属性是 //www.zgpingshu.com/play/4151/

post返回错误的地址是http://ops101.zgpingshu.com/%E5%8D%95%E7%94%B0%E8%8A%B3/%E5%A4%A7%E6%98%8E%E8%8B%B1%E7%83%88%28180%E5%9B%9E%2932kbps/67039B0D11.flv
把结尾的.flv替换成.mp3
'''
def getDownloadUrl(postUrl):
    success = False
    maxCount = 10
    current = 1
    while (not success and current < maxCount):
        try:
            r = requests.post(postUrl)
            wrongUrl = (r.json())["urlpath"]
            correctUrl = wrongUrl.replace(".flv", ".mp3")

            success = True
            return correctUrl
        except Exception as e:
            success = False
            print(postUrl + " 请求出错: " + str(e))
            
            current = current + 1
            print("等待3秒第次%d请求..." % current)
            time.sleep(3)

"""
缓存要下载的MP3文件名, 地址到json文件中
maxCount: 最多缓存个数, 可用于测试
refresh: true删除原文件重新更新

返回key为文件名, value为下载地址
"""
def cacheDownloadUrls(cachePath, maxCount = None, refresh = False):
        if (os.path.exists(cachePath) and os.path.isfile(cachePath)):
            if not refresh:
                print('读取缓存文件' + cachePath)
                return json.load(open(cachePath, 'r'))
            else:
                print('删除缓存文件' + cachePath)
                os.remove(cachePath)

        mainPage = getETree(playlistUrl)
        #播放列表页面中每个MP3播放地址
        urllist = mainPage.xpath("//div[@class='play-btns clearfix']/li/div[@class='player']/a/@href")

        print("读取到%d个文件" % len(urllist))
        
        downloadItems = {}
        logLines = []
        for (i, url) in enumerate(urllist):
            # max limit
            if (maxCount) and (i+1) > maxCount:
                break

            print("读取第%s个文件下载地址" % (i+1))

            postUrl = "http://www.zgpingshu.com/playdata/%d/" % playlistId
            # MP3地址post请求规则
            # 如果是第一个, index.html, http://www.zgpingshu.com/playdata/4151/index.html
            # 其余根据序号，如第二个 http://www.zgpingshu.com/playdata/4151/2.html
            if (i == 0):
                postUrl += "index.html"
            else:
                postUrl += "%d.html" % (i+1)

            # post方式获取MP3地址
            fileName = "%03d" % (i+1) + ".mp3"

            downloadItems[fileName] = getDownloadUrl(postUrl)

            '''
            html = getETree(correctDownloadUrl(postUrl)) # mp3播放页面地址
            #encryptDownloadUrl = html.xpath("//a[@id='down']/@href")
            encryptDownloadUrl = html.xpath("//audio[@id='jp_audio_0']/@src")
            if (len(encryptDownloadUrl) == 1):
                #downloadUrl = decryptDownloadUrl(encryptDownloadUrl[0])
                downloadUrl = encryptDownloadUrl[0]
                fileName = "%03d" % (i+1) + ".mp3"
                downloadItems[fileName] = downloadUrl
                logLines.append("%3d\t%s\n" % ((i+1), downloadUrl))
            '''

        print("共保存个%d文件下载地址" % len(downloadItems))
        f = open(cachePath, "w")
        json.dump(downloadItems, f)

        return downloadItems

'''
https://www.toptal.com/python/beginners-guide-to-concurrency-and-parallelism-in-python
https://github.com/volker48/python-concurrency/blob/master/threading_imgur.py
'''
class DownloadWorker(Thread):

    def __init__(self, queue):
        Thread.__init__(self)
        self.queue = queue

    def run(self):
        while True:
            # Get the work from the queue and expand the tuple
            directory, link, fileName = self.queue.get()
            try:
                ts = time.time()
                download_link(directory, link, fileName)
                print('下载 %s, 用时%s' % (fileName, round(endtime - starttime, 2)))
            finally:
                self.queue.task_done()

"""
下载文件
directory: 保存的文件夹
link: 下载地址
fileName: 保存的文件名
"""
def download_link(directory, link, fileName = None):
    if not fileName:
        fileName = os.path.basename(link)
    download_path = directory / fileName
    with download_path.open('wb') as f:
        r = requests.get(link)
        f.write(r.content)
    #logger.info('Downloaded %s', link)

"""
生成下载文件夹
"""
def setup_download_dir(dirName):
    download_dir = Path(dirName)
    if not download_dir.exists():
        download_dir.mkdir()
    return download_dir

#generateDownloadFile("test.csv")
def main():
    starttime = time.time()

    download_dir = setup_download_dir("mp3")

    downloadItems = cacheDownloadUrls("cache.urls.json")

    print("开始下载%d个文件" % len(downloadItems))

    # Create a queue to communicate with the worker threads
    queue = Queue()
    # Create 8 worker threads
    for x in range(8):
        worker = DownloadWorker(queue)
        # Setting daemon to True will let the main thread exit even though the workers are blocking
        worker.daemon = True
        worker.start()
    # Put the tasks into the queue as a tuple
    for (fileName, downloadUrl) in downloadItems.items():
        queue.put((download_dir, downloadUrl, fileName))
    #download_link(download_dir, "http://image.kaolafm.net/mz/audios/201603/619e0ba5-2ec9-430f-9635-52d1f9c3b561.mp3")
    # Causes the main thread to wait for the queue to finish processing all the tasks
    queue.join()

    endtime = time.time()
    print('总共的时间为:', round(endtime - starttime, 2) / 60, " 分钟")

if __name__ == '__main__':
    main()

