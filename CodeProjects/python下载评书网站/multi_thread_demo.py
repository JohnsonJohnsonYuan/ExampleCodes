
from queue import Queue
from threading import Thread
import time

"""
多线程例子: 参考 https://docs.python.org/2/library/queue.html#Queue.Queue.join

创建的多个Thread通过调用同一个Queue对象的get()方法来执行任务
默认get方法会一直等待直到Quque中有元素
例子中worker方法增加了当某个时间没有元素后该线程结束

get: get(block = False, timeout=None) Remove and return an item from the queue.
join: Blocks until all items in the queue have been gotten and processed.
      The count goes down whenever a consumer thread calls task_done() to indicate that the item was retrieved and all work on it is complete. When the count of unfinished tasks drops to zero, join() unblocks.

def worker():
    while True:
        item = q.get()
        do_work(item)
        q.task_done()

q = Queue()
for i in range(num_worker_threads):
     t = Thread(target=worker)
     t.daemon = True    # Setting daemon to True will let the main thread exit even though the workers are blocking
     t.start()

for item in source():
    q.put(item)

q.join()       # block until all tasks are done

"""

def worker(name, timeout = 2):
    hasItem = True
    while hasItem:
        try:
            print(name + ' waiting value')
            item = q.get(False, timeout)
            print('%s get value %s' % (name, item))
            time.sleep(3)
            #do_work(item)
            print('%s processed value %s' % (name, item))
            q.task_done()
        except :
            hasItem = False
        finally:
            print(name + " thread is done")

num_worker_threads = 3
q = Queue()

threads = []
for i in range(num_worker_threads):
     t = Thread(target=worker, kwargs={"name": str(i)})
     t.daemon = True
     t.start()
     threads.append(t)

for item in range(10):
    q.put(item)

q.join()       # block until all tasks are done

print('stop')
for t in threads:
    t.join()

print('stopped')
