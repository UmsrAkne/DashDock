## 設定 (Configuration)

本アプリの Memos エリアを使うには接続情報（Base URL および Access Token）の設定が必要です。
以下のいずれかの方法で設定を行ってください。

### 方法1: `.env` ファイルを使用する（推奨）

1. プロジェクトルート（または実行ファイルと同じディレクトリ）にある `.env.example` をコピーして `.env` を作成します。
2. `.env` をエディタで開き、各値を設定します。

```env
MEMOS_BASE_URL=https://your-memos-instance.com
MEMOS_TOKEN=your_access_token_here
```

### 方法2: OSの環境変数を設定する
`.env` ファイルと同じ変数名で環境変数に値をセットします。