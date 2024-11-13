# import socket

# # サーバーのIPアドレスとポート番号
# IPADDR = "127.0.0.1"
# PORT = 49152

# # クライアントソケットの作成
# sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# # サーバーへ接続
# sock.connect((IPADDR, PORT))

# while True:
#     # ユーザーからの入力を受け取る
#     message = input("サーバーに送信するメッセージを入力してください（終了するには'quit'と入力）：")

#     # 'quit' と入力された場合、接続を終了
#     if message.lower() == 'quit':
#         print("接続を終了します...")
#         sock.sendall(b"END")  # サーバーに終了のシグナルを送信
#         break

#     # 入力されたメッセージをサーバーに送信
#     sock.sendall(message.encode('utf-8'))

#     # サーバーからのレスポンスを受け取って表示
#     server_response = sock.recv(1024)
#     print(f"サーバーからのレスポンス: {server_response.decode('utf-8')}")

# # ソケットを閉じる
# sock.close()

import socket

# サーバーのIPアドレスとポート番号
IPADDR = "127.0.0.1"
PORT = 49152

# クライアントソケットの作成
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# サーバーへ接続
sock.connect((IPADDR, PORT))

while True:
    # ユーザーからの入力を受け取る
    message = input("サーバーに送信するメッセージを入力してください（終了するには'quit'と入力）：")

    # 'quit' と入力された場合、接続を終了
    if message.lower() == 'quit':
        print("接続を終了します...")
        sock.sendall(b"END")  # サーバーに終了のシグナルを送信
        break

    # 入力されたメッセージをサーバーに送信
    sock.sendall(message.encode('utf-8'))

    # サーバーからのレスポンスを受け取って表示
    server_response = sock.recv(1024)  # サーバーから最大1024バイトを受信
    print(f"サーバーからのレスポンス: {server_response.decode('utf-8')}")

# ソケットを閉じる
sock.close()
