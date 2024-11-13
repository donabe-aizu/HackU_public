# HackU

## 作業フォルダ
各自の名前のフォルダを作って作業しよう

例: Assets/donabe

## ブランチについて

- メインの最新データがある場所 → main  
- 各自の作業 → 名前/作業内容
  - 例: donabe/player_move
  
## GitとGithubを使った作業の進め方

1. 最新のmainをpullする  
   ![image](https://github.com/user-attachments/assets/840d0718-6ea9-4157-a2d1-b7541a3d7658)
2. mainからブランチを切って、そのブランチに移動する  
   ![image](https://github.com/user-attachments/assets/fb8f9e36-e944-4e64-a2dc-447d83193421)
3. 自分の名前のフォルダ内で作業をする
4. LacalChangeで変更したファイルをStageする  
   ![image](https://github.com/user-attachments/assets/ca20bb8c-b6b4-4d4d-a240-1350ec7ff2ee)
5. コミットメッセージを書いて、コミット
   ![image](https://github.com/user-attachments/assets/e7faefcd-d0a7-415f-bfd1-c2ab491504b1)
6. ブランチをPush（一回目はリモートにブランチがCreateされる）
   ![image](https://github.com/user-attachments/assets/1a94511a-62d6-420e-ae7b-31a34e0bdef5)
7. 作業が一段落（一機能が完成）するまで3から6を繰り返す
8. 作業が一段落したらブランチの名前を右クリックして、Create Pull requestを押す
   ![image](https://github.com/user-attachments/assets/dcc14283-ca28-4583-9599-383528a3a082)
9. Githubのページが開いたら、プルリクエストのタイトル等を書く
   ![image](https://github.com/user-attachments/assets/d8fc5975-2dad-4f5c-ab1b-2610b574678e)
10. コンフリクトが発生していなかったらマージ(もしコードに不安があったら、この状態で誰かからレビューをもらう)
    ![image](https://github.com/user-attachments/assets/a9faab0c-903c-4ea6-b7c8-7b267f77e8e7)
11. mainにマージされて自分の変更が反映される。

## Gitの注意点

- Unityは自動生成・変更のファイルが多いので、自分が変更したファイルじゃない場合はチェック
- mainに直接Pushしない


