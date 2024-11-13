import copy
import cv2
import json
import os
import numpy as np
import mediapipe as mp
from datetime import datetime
from ultralytics import YOLO
from collections import deque
from tensorflow.keras.models import load_model
import socket

###初期値

# プレス検出カウンターと状態の初期化
thumb_movement_history = deque(maxlen=10)  # 10フレーム分の変動量を保存
cumulative_threshold = 0.3  # 10フレームの合計変動量の閾値（適切に調整してください）
press_counter = 0  # 連続で押して離す動作をカウント

# YOLOの検出状態を追跡するためのカウンター

stable_detection_count = 0
required_stable_frames = 20  # 必要な安定フレーム数
confidence_threshold = 0.6  # 信頼度の閾値
# フレーム数カウンターと骨格推定結果の保存
frame_count = 0
landmark_history = []  # 骨格推定結果を格納するリスト



# サーバーのIPアドレスとポート番号
IPADDR = socket.gethostbyname(socket.gethostname())
PORT = 49152



# サーバーソケットの作成とバインド
server_sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_sock.bind((IPADDR, PORT))
server_sock.listen(1)

print(f"サーバーを起動しました。IP: {IPADDR}, ポート: {PORT}")

# クライアントからの接続を待機
client_sock, client_addr = server_sock.accept()
print(f"クライアントが接続されました: {client_addr}")






# YOLOモデルの初期化
yolo_model = YOLO("best.pt").to("cpu")
cap = cv2.VideoCapture(1)

# Mediapipe Handモデルの初期化
mp_hands = mp.solutions.hands
hands = mp_hands.Hands(max_num_hands=1, min_detection_confidence=0.7, min_tracking_confidence=0.5)
mp_drawing = mp.solutions.drawing_utils  # ランドマーク描画用のユーティリティ

# # JSONファイルに数値のリストとしてデータを書き込む関数
# def write_to_json(file_path, data):
#     os.makedirs("result", exist_ok=True)
#     with open(file_path, "w") as json_file:
#         json.dump(data, json_file, indent=4)

# # JSONファイルを読み込んでデータを配列に格納
# def load_hand_landmarks(file_path):
#     with open(file_path, 'r') as f:
#         data = json.load(f)
#     return data



def use_model(keypoint):

    global frame_count

    # テストデータの読み込み
    info = []
    hand_landmarks_data = keypoint  # `keypoint` データを直接使用

    # `keypoint` データの解析と `info` へのデータ格納
    for frame_data in hand_landmarks_data:
        trush = []
        for hand_data in frame_data:
            for point in hand_data:
                trush.extend([point['x'], point['y']])  # x, yを追加
        info.append(trush)

    info = np.array(info)  # infoをnumpy配列に変換

    # 予測
    predicted_y = model.predict(info)

    # 結果を表示
    for i in range(len(predicted_y)):  
        class_prediction = np.argmax(predicted_y[i])  # 最大確率を持つクラスを取得

    frame_count = 0  # フレームカウントリセット
    # 予測結果の中で最も多かったクラスを出力するように変更

    # 予測結果を保存するリスト
    predicted_classes = []

    # 予測結果に基づいて多かったクラスをカウント
    for i in range(len(predicted_y)):  
        # 最大確率を持つクラスを取得
        class_prediction = np.argmax(predicted_y[i])  
        predicted_classes.append(class_prediction)

    # クラスの出現回数をカウント
    class_counts = {0: 0, 1: 0}

    for class_prediction in predicted_classes:
        class_counts[class_prediction] += 1

    # 出現回数が多かったクラスを選択
    if class_counts[0] > class_counts[1]:
        most_common_class = 0
    else:
        most_common_class = 1

    # 出力
    print(f"Most common class: {most_common_class}")

    if most_common_class==1:
        data_to_save = {"fox": most_common_class}


        json_data = json.dumps(data_to_save)


        client_sock.sendall(json_data.encode('utf-8'))  # UTF-8エンコードで送信
           
        print("fox をクライアントに送信しました。")



# プレス検出関数（親指の押しと離しの動作を確認）
def detect_press(frame):
    global press_counter

    frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    results = hands.process(frame)

    if results.multi_hand_landmarks is not None:
        for hand_landmarks in results.multi_hand_landmarks:
            # 親指のy座標と手首のy座標の相対位置を取得
            thumb_y = hand_landmarks.landmark[4].y
            wrist_y = hand_landmarks.landmark[0].y
            relative_thumb_y = thumb_y - wrist_y

            # 親指の変動量を計算
            if 'prev_thumb_y' in globals():
                thumb_y_change = abs(relative_thumb_y - prev_thumb_y)

                # 10フレームの変動量を記録
                thumb_movement_history.append(thumb_y_change)

                # 10フレームごとの合計変動量を計算
                if len(thumb_movement_history) == 10:
                    cumulative_movement = sum(thumb_movement_history)

                    # 合計変動量が閾値を超えたら押下とみなす
                    if cumulative_movement > cumulative_threshold:
                        press_counter += 1

                        # 2回連続で検知されたら出力
                        if press_counter == 3:
                            press_counter = 0  # カウンターをリセット
                            print("Press Detection: 1")
                            data_to_save = {"press": 1}
                            json_data = json.dumps(data_to_save)
                            client_sock.sendall(json_data.encode('utf-8'))  # UTF-8エンコードで送信
                            print("Pressをクライアントに送信しました。")


            # 現在の親指の位置を次フレーム用に保存
            globals()['prev_thumb_y'] = relative_thumb_y



def output_result(question, class_id):
    global message
    if question == 1:
        if class_id == 5:
            data_to_save = {"question1": 1}
            json_data = json.dumps(data_to_save)
            client_sock.sendall(json_data.encode('utf-8'))  # UTF-8エンコードで送信
            print("question1 をクライアントに送信しました。")
            message=100

        elif class_id in [0, 3]:
            data_to_save = {"question1": 0}
            json_data = json.dumps(data_to_save)
            client_sock.sendall(json_data.encode('utf-8'))  # UTF-8エンコードで送信
            print("question1 をクライアントに送信しました。")


    elif question == 2:
        if class_id == 6:
            data_to_save = {"question2": 1}
            json_data = json.dumps(data_to_save)
            client_sock.sendall(json_data.encode('utf-8'))  # UTF-8エンコードで送信
            print("question2 をクライアントに送信しました。")
            message=100



        elif class_id in [1, 2]:
            data_to_save = {"question2": 0}
            json_data = json.dumps(data_to_save)
            client_sock.sendall(json_data.encode('utf-8'))  # UTF-8エンコードで送信
            print("question2 をクライアントに送信しました。")

    elif question == 3:
        if class_id == 8:
            data_to_save = {"question3": 1}
            json_data = json.dumps(data_to_save)
            client_sock.sendall(json_data.encode('utf-8'))  # UTF-8エンコードで送信
            print("question3 をクライアントに送信しました。")
            message=100

        elif class_id in [4, 7]:
            data_to_save = {"question3": 0}
            json_data = json.dumps(data_to_save)
            client_sock.sendall(json_data.encode('utf-8'))  # UTF-8エンコードで送信
            print("question3 をクライアントに送信しました。")



# メインループ
highest_confidence_class = None  # 前フレームでの最も信頼度の高いクラス
# モデルの読み込み
model = load_model("fox_model_light.keras", compile=False)

question=0
message=100
while True:
    # FPS計測用に時間を記録
    start_time = cv2.getTickCount()
###Unity側から受信する問題の場所


    ret, frame = cap.read()
    if not ret:
        break
    if message==100:
        message = client_sock.recv(1024).decode('utf-8')

    if message:  # メッセージが空でない場合
    
        question=int(message)
      
        # YOLO検出
        # 何もないとき
        if question==0:
            results = yolo_model.predict(source=frame, verbose=False)
        # 乗り物
        if question==1:
            results = yolo_model.predict(source=frame, conf=confidence_threshold, verbose=False,classes=[0,3,5])

        # 色    
        if question==2:
            results = yolo_model.predict(source=frame, conf=confidence_threshold, verbose=False,classes=[1,2,6])
        # 塗るもの
        if question==3:
            results = yolo_model.predict(source=frame, conf=confidence_threshold, verbose=False,classes=[4,7,8])
        #ラスト
        if question==4:
            results = yolo_model.predict(source=frame, conf=confidence_threshold, verbose=False,classes=[8])

        # 現フレームで最も信頼度の高いクラスを探す
        frame_highest_confidence_class = None
        highest_confidence = 0.0
        for box in results[0].boxes:
            confidence = box.conf.numpy().item()
            cls = int(box.cls.numpy().item())
            if confidence > highest_confidence:
                highest_confidence = confidence
                frame_highest_confidence_class = cls

        # クラスの安定検出をチェック
        if frame_highest_confidence_class == highest_confidence_class and highest_confidence >= confidence_threshold:
            stable_detection_count += 1
        else:
            stable_detection_count = 0  # カウンターをリセット

        highest_confidence_class = frame_highest_confidence_class

        # 一定フレーム数にわたり安定した検出結果が得られた場合のみ保存・出力
        if stable_detection_count >= required_stable_frames and highest_confidence_class is not None:
            output_result(question,highest_confidence_class)
            stable_detection_count = 0  # 出力後にカウンターをリセット
        annotated_frame = results[0].plot()
        cv2.imshow("YOLO Detection", annotated_frame)


        
        if question == 4:
            # プレス検出
            detect_press(frame)

        # YOLO検出結果の表示

        if question not in [0, 4]:
            # Mediapipe Handランドマークの描画
            frame_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            hand_results = hands.process(frame_rgb)

            # 手のランドマークが検出されていれば描画
            if hand_results.multi_hand_landmarks:
                for hand_landmarks in hand_results.multi_hand_landmarks:
                    mp_drawing.draw_landmarks(frame, hand_landmarks, mp_hands.HAND_CONNECTIONS)

                # 骨格推定のランドマークを保存
                frame_landmarks = []
                for hand_landmarks in hand_results.multi_hand_landmarks:
                    hand_data = []
                    for landmark in hand_landmarks.landmark:
                        hand_data.append({
                            'x': landmark.x,
                            'y': landmark.y,
                            'z': landmark.z
                        })
                    frame_landmarks.append(hand_data)
                
                # ランドマークデータをフレームごとに格納
                landmark_history.append(frame_landmarks)

                # 30フレームごとに保存
                frame_count += 1
                if frame_count >= 20:
                    use_model(landmark_history)
                    landmark_history = []

        # FPS計算
        elapsed_time = (cv2.getTickCount() - start_time) / cv2.getTickFrequency()
        fps = 1 / elapsed_time

        # FPSを画面に表示
        cv2.putText(annotated_frame, f"FPS: {fps:.2f}", (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)
        cv2.imshow("YOLO Detection", annotated_frame)

        # 「q」キーで終了
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    else:  # メッセージが空の場合は question を 1 と設定（デフォルト）
        pass




client_sock.sendall(b"END")  # ファイル送信の終わりを知らせる
print("サーバー終了")

cap.release()
cv2.destroyAllWindows()
# ソケットを閉じる
client_sock.close()
server_sock.close()

