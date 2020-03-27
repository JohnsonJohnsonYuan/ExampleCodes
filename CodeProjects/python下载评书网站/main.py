import requests
from lxml import etree
import os
from queue import Queue
from pathlib import Path
from time import time
from threading import Thread
import json

"""
下载评书网站https://www.ishuyin.com/下某个专辑下所有MP3
用到的包:
    requests: 请求返回html
    lxml: xpath 查找

getDownloadItems方法中配置要下载的播放列表
"""

def getContent(url):
    r = requests.get(url)
    r.encoding = "utf-8"
    return r.text

def getETree(url):
    text = getContent(url)
    return etree.HTML(text)

'''
返回正确MP3下载地址
ishuyin 网站的下载地址默认是unicode的int值，需要转换
'*104*116*116*112*58*47*47*105*109*97*103*101*46*107*97*111*108*97*102*109*46*110*101*116*47*109*122*47*97*117*100*105*111*115*47*50*48*49*54*48*51*47*51*102*55*98*100*52*53*97*45*52*48*56*102*45*52*56*53*100*45*98*56*100*50*45*98*102*55*48*57*55*50*99*57*101*99*52*46*109*112*51*'
'''
def decryptDownloadUrl(url):
    items = url.split('*')
    result = ''
    for i in items:
        if len(i) > 0:
            result += chr(int(i))
    return result

"""
解析要下载的MP3
"""
def getDownloadItems(cachePath, refresh = False):
        if (os.path.exists(cachePath) and os.path.isfile(cachePath)):
            if not refresh:
                return json.load(open(cachePath, 'r'))
            else:
                os.remove(cachePath)

        mainPage = getETree("https://www.ishuyin.com/show-1823.html")
        urllist = mainPage.xpath("//div[@class='box'][2]/a/@href")
        
        downloadItems = {}
        logLines = []
        for (i, url) in enumerate(urllist):
            print("保存第%s个文件下载地址" % (i+1))
            html = getETree('https://www.ishuyin.com/' + url)
            encryptDownloadUrl = html.xpath("//a[@id='urlDown']/@href")
            if (len(encryptDownloadUrl) == 1):
                downloadUrl = decryptDownloadUrl(encryptDownloadUrl[0])
                fileName = "%03d" % (i+1) + ".mp3"
                downloadItems[fileName] = downloadUrl
                logLines.append("%3d\t%s\n" % ((i+1), downloadUrl))

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
                ts = time()
                download_link(directory, link, fileName)
                print('下载 %s, 用时%s' % (fileName, time() - ts))
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
    logger.info('Downloaded %s', link)

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
    ts = time()

    download_dir = setup_download_dir("mp3")

    downloadItems = getDownloadItems("cache.urls.json")
    
    # Create a queue to communicate with the worker threads
    queue = Queue()
    # Create 8 worker threads
    for x in range(8):
        worker = DownloadWorker(queue)
        # Setting daemon to True will let the main thread exit even though the workers are blocking
        worker.daemon = True
        worker.start()
    # Put the tasks into the queue as a tuple
    for (name, url) in downloadItems.items():
        queue.put((download_dir, url, name))
    #download_link(download_dir, "http://image.kaolafm.net/mz/audios/201603/619e0ba5-2ec9-430f-9635-52d1f9c3b561.mp3")
    # Causes the main thread to wait for the queue to finish processing all the tasks
    queue.join()
    print('总用时 %s'.format(name))

if __name__ == '__main__':
    main()