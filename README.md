# Dynamics CRM 活動報告ボット サンプル

Microsoft Bot Framework と Microsoft Dynamics CRM で連携をするサンプルです。  
Dynamics CRM のユーザーのその日の訪問予定を認識し、その内容を更新するボットです。

## 動作確認環境
* Visual Studio 2015 Update 3
* Bot Framework v3.0
* Dynamics CRM Online 2016
  
  
## 前提
Dynamics CRM の予定エンティティに以下のようなカスタムフィールドが存在している想定です。  

* 名前：次のアクション  
* 内部名：new_nextaction  
* 種類：オプションセット  
* オプションセットのオプション：  
    * <table>
    <tr>
        <td>ラベル</td>
        <td>値</td>
    </tr>
    <tr>
        <td>ヒアリング</td>
        <td>100000000</td>
    </tr>
    <tr>
        <td>提案</td>
        <td>100000001</td>
    </tr>
    <tr>
        <td>見積もり</td>
        <td>100000002</td>
    </tr>
    <tr>
        <td>交渉</td>
        <td>100000003</td>
    </tr>
    <tr>
        <td>契約</td>
        <td>100000004</td>
    </tr>
</table>
  
なお、ボットはそのユーザーのその日の予定レコードのうち説明フィールドの値が空のものを取得するようになっています。  
  
  
また、ユーザーエンティに以下のようなカスタムフィールドが存在している想定です。このフィールドには、Bot Connector から渡される Activity の From.Name の値を保持します。例えば、Skype からこのボットを利用している場合には、 Skype のユーザー名 の値がこれに相当します。  
また、代替キーとして、このフィールドのみを含むキーが設定されていることが必要です。
* 名前：BotUserId
* 内部名：new_botuserid  
* 種類：１行テキスト  
  
  
なお、このサンプルではボットが Dynamics CRM にアクセスするために固定的なユーザー クレデンシャルにてアクセスしています。そのユーザーは他のユーザーが所有するレコードにアクセスしますので、セキュリティロール「代理人」に含まれるような prvActOnBehalfOfAnotherUser 特権を保持している必要があります。
  
  
実際に利用される Dynamics CRM や Bot 環境に合わせて、Settings.cs と Web.config を適宜修正ください。
