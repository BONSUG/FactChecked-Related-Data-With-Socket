import socket
import time
import threading

#데이터 관련
import pandas as pd

# new_post 와 contents의 거리 구하기
import scipy as sp

from sklearn.feature_extraction.text import TfidfVectorizer
from konlpy.tag import Okt


out_data = ""
in_data=""
#출력데이터
df = pd.read_csv("factcheck5.csv")  # load data
sp.log(3 / 3)
sp.log(3 / 2)
sp.log(3 / 1)


def tfidf(t, d, D):
    tf = float(d.count(t)) / sum(d.count(w) for w in set(d))
    idf = sp.log(float(len(D)) / len([doc for doc in D if t in doc]))
    return tf, idf

def dist_raw(v1, v2):
    delta = v1 - v2
    return sp.linalg.norm(delta.toarray())

def search(in_data):
    if(in_data==""):
        return 0
    # 데이터 처리
    t = Okt()
    contents = df["topic"]
    vectorizer = TfidfVectorizer(min_df=1, decode_error='ignore')
    contents_tokens = [t.morphs(row) for row in contents]

    contents_for_vectorize = []

    for content in contents_tokens:
        sentence = ''
        for word in content:
            sentence = sentence + ' ' + word

        contents_for_vectorize.append(sentence)

    X = vectorizer.fit_transform(contents_for_vectorize)
    num_samples, num_features = X.shape

    new_post = [in_data]
    new_post_tokens = [t.morphs(row) for row in new_post]

    new_post_for_vectorize = []

    for content in new_post_tokens:
        sentence = ''
        for word in content:
            sentence = sentence + ' ' + word

        new_post_for_vectorize.append(sentence)

    # transform
    new_post_vec = vectorizer.transform(new_post_for_vectorize)
    # 다른 결과를 얻을 수 있음
    best_doc = None
    best_dist = 65535
    best_i = None

    for i in range(0, num_samples):
        post_vec = X.getrow(i)

        # 함수호출
        d = dist_raw(post_vec, new_post_vec)

        # print("== Post %i with dist=%.2f   : %s" %(i,d,contents[i]))

        if d < best_dist:
            best_dist = d
            best_i = i

    print("Best post is %i, dist = %.2f" % (best_i, best_dist))
    print('-->', new_post)
    print('---->', contents[best_i])

    idx = df[df['topic'] == contents[best_i]].index
    out_data = df.loc[idx]
    rate = int(out_data['accuracy'].values)
    rate = str(rate)
    temp = str(out_data['topic'].values + "@" + out_data['type'].values + "@" + rate + "@" + out_data['link'].values)

    return temp

def send(sock):                 # 데이터 송신 함수
    while True:
        global out_data
        out_data = search(in_data)
        if(out_data==0):
            out_data=""
        if(out_data!=""):
            # 출력
            tem=out_data
            out_data=tem.encode("utf-8")
            sock.send(out_data)
            print('송신 데이터 :', tem)

def recv(sock):                 # 데이터 수신 함수
    while True:
        global in_data
        in_data = sock.recv(1024*7).decode("utf-8")
        if(in_data!=""):
            print(in_data)
            print("수신 데이터 :  ", in_data)

host = '192.168.219.100' # 호스트 ip를 적어주세요
port = 6000           # 포트번호를 임의로 설정해주세요

server_sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_sock.bind((host, port))
server_sock.listen(1)
print("기다리는 중..")

client_sock, addr = server_sock.accept()  # 연결 승인

if client_sock:
    print('Connected by ', addr) #연결주소

    sender = threading.Thread(target=send, args=(client_sock,))  # 송신 쓰레드
    receiever = threading.Thread(target=recv, args=(client_sock,))  # 수신 쓰레드

    sender.start()
    receiever.start()


    while True:
        time.sleep(1)

        pass
