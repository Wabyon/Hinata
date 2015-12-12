Hinata
======
[現在はオンプレミス版を主体として開発を進めています。](https://github.com/Wabyon/Hinata-on-premise)
****

HinataはASP.NET MVCで作成されたMS Azureで動作する[Qiita](http://qiita.com/)クローンです。

### このプロジェクトについて
企業内で使用することを前提としたQiitaのクローンです。

### 主な機能
* アカウントの登録をメールアドレスのドメインで限定
* SendGridによるアカウント登録確認メール
* Markdownによる記事の投稿と閲覧
* GFM(GitHub Flavored Markdown)に一部対応
* 記事の下書き保存
* 記事の限定公開
* 画像の登録

### 動作環境
* Azure Websites
* Azure SQL Database
* Azure Storage

アカウントの管理、記事の保存はAzure SQL Databaseで行っています。
画像の保存はAzure Storageで行っています。

### 開発環境、言語
C# + ASP.NET MVC 5で開発しています。
開発環境はVisual Studio 2013 Profesionalです。

### ライブラリ
* jQuery
* marked.js
* highlight.js
* Modernizr
* Twitter Bootstrap
* [github-markdown-css](https://github.com/sindresorhus/github-markdown-css)
* Dapper
* JavaScriptEngineSwitcher.Core
* JavaScriptEngineSwitcher.V8
* ClearScript
* Unity
* WebActivatorEx
* HtmlAgilityPack
* WebGrease
* Newtonsoft.Json
* Owin
* Microsoft.AspNet.Identity
* Microsoft.AspNet.Mvc
* Microsoft.AspNet.Razor
* Microsoft.AspNet.Web.Optimization
* Microsoft.AspNet.WebPages
* WindowsAzure.Storage

### ライセンス
* MITライセンス

MITライセンスの下で公開する、オープンソースソフトウェアです。
