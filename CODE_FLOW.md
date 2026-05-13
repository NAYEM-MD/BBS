# CODE_FLOW.md — w2.BBS.Front プロジェクト解説

> Project: `c:\inetpub\wwwroot\Training.BBS\Web\w2.BBS.Front`
> Stack: ASP.NET MVC 5 (.NET Framework 4.8) + Liquid テンプレート + Vue 3 + SQL Server (`bbs.mdf`)
> Goal of this doc: এক জায়গায় বুঝা — কোন ফাইল কী করে, কে কাকে কল করে, কোন SQL/DB টেবিলে যাচ্ছে, কোথায় ডুপ্লিকেট/একই রকম মেথড আছে।

---

## 1. リクエスト全体の流れ (Request lifecycle)

```
Browser
  │  HTTP GET/POST
  ▼
IIS  ──►  w2.BBS.Front (ASP.NET MVC 5)
                │
                ├─ Global.asax           ⇒ Application_Start → RouteConfig.RegisterRoutes
                ├─ App_Start/RouteConfig ⇒ MapMvcAttributeRoutes() で各 Controller の [Route] を有効化
                ├─ App_Start/UnityConfig + UnityMvcActivator
                │     ⇒ Unity DI コンテナを MVC に注入
                ├─ Controller            ⇒ AuthController / AccountController / ForumController / IndexController
                │     ↳ BaseController.View(...)        ⇒ Liquid テンプレートで HTML 返す
                │     ↳ BaseController.JsonForJs(...)   ⇒ JSON を返す（fetch 用）
                ▼
DB (bbs.mdf):  w2_User / w2_ForumPost / w2_ForumReply / w2_Sample
```

* Liquid を描画するのは `BaseController.View()` 内の `FluidRenderer`。
* fetch API（Vue から）→ `[Route("~/api/...")]` のエンドポイント → `JsonForJs` で JSON 返却。
* セッション (`SESSION_KEY_LOGIN_USER_ID` 等) でログイン状態を保持。

---

## 2. プロジェクト構成 (Folder map)

```
Web/w2.BBS.Front/
├─ Global.asax / Global.asax.cs        → アプリ起動エントリ。RouteConfig を呼ぶ。
├─ Web.config                           → IIS 設定、connection string 等。
├─ App_Start/
│   ├─ RouteConfig.cs                   → ルーティング登録（属性ルート有効化）。
│   ├─ UnityConfig.cs                   → Unity コンテナ生成。
│   └─ UnityMvcActivator.cs             → Unity を MVC DependencyResolver に登録。
│
├─ Controller/
│   ├─ Shared/BaseController.cs         → View() / JsonForJs() を共通実装。
│   ├─ IndexController.cs               → "/" トップ + サンプル "/get-message" "/update-message"。
│   ├─ AuthController.cs                → ログイン / 会員登録 / ログアウト / DB-check。
│   ├─ AccountController.cs             → 設定画面、名前・パスワード変更、退会。
│   └─ ForumController.cs               → 掲示板 (一覧/作成/編集/削除/返信)。
│
├─ ViewModels/
│   ├─ BaseViewModel.cs                 → RootUrl 共通プロパティ。
│   ├─ IndexViewModel.cs                → トップ画面用。
│   ├─ AuthLoginViewModel.cs            → login.liquid 用。
│   ├─ AuthRegisterViewModel.cs         → register.liquid 用。
│   ├─ AccountSettingsViewModel.cs      → settings.liquid 用。
│   ├─ ForumIndexViewModel.cs           → forum.liquid 初期表示用。
│   ├─ ForumPostViewModel.cs            → 投稿 1 件 (Replies[] を含む)。
│   ├─ ForumReplyViewModel.cs           → 返信 1 件。
│   ├─ ForumListResponseViewModel.cs    → /api/forum/posts のレスポンス。
│   └─ ForumActionResponseViewModel.cs  → 作成/編集/削除/返信レスポンス。
│
├─ Views/
│   ├─ _Shared/layout.liquid            → 共通レイアウト (Vue/Vuetify/jQuery 読込み, 共通 mount)。
│   ├─ index.liquid                     → トップ。
│   ├─ login.liquid                     → ログイン画面。
│   ├─ register.liquid                  → 会員登録画面。
│   ├─ settings.liquid                  → アカウント設定画面 + 退会フォーム。
│   └─ forum.liquid                     → 掲示板 (Vue コンポーネント "vue-forum")。
│
├─ scripts/vue_manager.js               → 全画面共通の Vue ルート管理クラス VueManager。
└─ Codes/
    ├─ Helper/TempDataDictionaryExtension.cs  → TempData の型付き拡張。
    └─ Binding/                                → エイリアスバインダ（モデル属性で別名）。
```

---

## 3. ファイル間の接続 (Who calls who)

### 3.1 起動時
* `Global.asax` `Application_Start`
  → `Constants.APPLICATION_NAME = "w2.BBS.Front"` (`w2.Common`)
  → `RouteConfig.RegisterRoutes()`
* `UnityMvcActivator.Start` (アセンブリ属性で自動実行)
  → `UnityConfig.Container` → `DependencyResolver.SetResolver(...)`

### 3.2 共通レイアウト
* 各 `*.liquid` の先頭で `{% _layout 'layout.liquid' %}`
* `layout.liquid` が:
  * Vue / Vuetify / jQuery / mitt を CDN から読み込む
  * `scripts/vue_manager.js` を読込む（→ `VueManager` クラス）
  * `{% _render_area component %}` で各画面のコンポーネント定義を差し込む
  * `DOMContentLoaded` で `g_vueManager = new VueManager()` → 各画面の `{% _javascript key: "onload" %}` を実行 → `g_vueManager.mount()`

### 3.3 画面別フロー

#### A) トップ `IndexController.Index()` → `index.liquid`
* GET `/` → `IndexController` が `IndexViewModel` を `index.liquid` に渡す。
* JS: `vue-index` コンポーネント → `fetch('/get-message')` → `IndexController.GetMessage()` → SqlKata で `w2_Sample` を SELECT。
* JS: 「DB更新」 → `fetch('/update-message')` → `IndexController.UpdateMessage()` → SqlKata で `w2_Sample` を UPDATE。
* 注: `UpdateMessage2()` は SqlStatement (XML SQL) のサンプル。実画面からは未使用。

#### B) 認証 (Auth)
| ルート | メソッド | View |
|--------|----------|------|
| `GET /auth/login` | `AuthController.Login()` | `login.liquid` |
| `POST /auth/login` | `AuthController.Login(model)` | OK→ `redirect /forum`, NG→`login.liquid` |
| `GET /auth/register` | `AuthController.Register()` | `register.liquid` |
| `POST /auth/register` | `AuthController.Register(model)` | OK→`redirect /auth/login?registered=1` |
| `POST /auth/logout` | `AuthController.Logout()` | `redirect /auth/login` |
| `GET /auth/db-check` | `AuthController.DbCheck()` | text plain |

セッション登録/破棄は `Session[SESSION_KEY_LOGIN_USER_ID]` / `Session[SESSION_KEY_LOGIN_USER_NAME]`。

#### C) アカウント設定 / 退会 (Account)
| ルート | メソッド | View |
|--------|----------|------|
| `GET /account/settings` | `AccountController.Settings()` | `settings.liquid` |
| `POST /account/update` | `AccountController.Update(model)` | `settings.liquid` (再表示) |
| `POST /account/delete` | `AccountController.Delete(model)` | OK→`redirect /auth/login` |

* ログインID (`login_id`) は **表示のみ／変更不可**（ユニーク維持）。
* パスワード変更は **現在のパスワード必須**。
* 退会は **パスワード必須** + 自分の `w2_ForumPost`, `w2_ForumReply` を `del_flg=1` (Transaction)。

#### D) 掲示板 (Forum)
| ルート | メソッド | 形式 |
|--------|----------|------|
| `GET /forum` | `ForumController.Index()` | `forum.liquid` |
| `GET /api/forum/posts?page=N` | `ForumController.Posts(page)` | JSON |
| `POST /api/forum/create` | `ForumController.Create(title, body)` | JSON |
| `POST /api/forum/update` | `ForumController.Update(postId, title, body)` | JSON |
| `POST /api/forum/delete` | `ForumController.Delete(postId)` | JSON |
| `POST /api/forum/reply` | `ForumController.CreateReply(postId, body)` | JSON |

`forum.liquid` の Vue コンポーネント `vue-forum` がこれらの API を fetch で呼び、結果で再描画。

---

## 4. ファイル別 役割 (What each file means)

### 4.1 起動・共通基盤
| File | 役割（短文） |
|------|----------------|
| `Global.asax.cs` | アプリ開始時のフック。ルート登録、ログ出力先のパス設定。`Session_Start` でセッション ID 確定用ダミー値設定。 |
| `App_Start/RouteConfig.cs` | `MapMvcAttributeRoutes()` で `[Route]` を有効化。未定義ルートは `Error/RedirectShortUrlOrDisplay404` に飛ばす（実装は他プロジェクト想定）。 |
| `App_Start/UnityConfig.cs` | Unity コンテナ生成。`DiContainerConfiguratorEntryPoint` で各ドメインの DI を登録。 |
| `App_Start/UnityMvcActivator.cs` | Unity を ASP.NET MVC のフィルター/リゾルバに統合。WebActivatorEx で自動実行。 |
| `Codes/Helper/TempDataDictionaryExtension.cs` | `TempData` を `TempDataKey` Enum で型安全に扱う拡張。`AntiCsrfFormToken` キーが定義済み。 |
| `Codes/Binding/BindAliasAttribute.cs` | プロパティへ `[BindAlias("X")]` 付与で別名バインドを可能に。 |
| `Codes/Binding/AliasModelBinder.cs` | 上記別名バインドを実現する `DefaultModelBinder` 派生 + Enum を `EnumApiName` で受け取れる仕組み。 |
| `Codes/Binding/AliasedPropertyDescriptor.cs` | エイリアス用 PropertyDescriptor 実装。 |

### 4.2 共通コントローラ
| Member of `BaseController` | 役割 |
|----|---|
| `View(viewFileVirtualPath, model)` | Liquid テンプレートを `FluidRenderer` で描画して `ContentResult` を返す。`UTF-8`、`text/html`。`AntiCsrfFormToken` を `optionData` に渡す。 |
| `JsonForJs(obj)` | `Newtonsoft.Json` で JSON 化し `ContentResult("application/json", UTF-8)` を返す。`fetch` 側で `response.json()` 受け取り。 |

### 4.3 ViewModel (data shape)
| ViewModel | 用途 / 主なフィールド |
|---|---|
| `BaseViewModel` | `RootUrl` (FrontRootPath) を共通供給。 |
| `IndexViewModel` | `Message`。 |
| `AuthLoginViewModel` | `LoginId, Password, ErrorMessage, InfoMessage`。 |
| `AuthRegisterViewModel` | `LoginId, Password, UserName, ErrorMessage`。 |
| `AccountSettingsViewModel` | `LoginId(表示), UserName, CurrentPassword, NewPassword, NewPasswordConfirm, DeletePassword, SuccessMessage, ErrorMessage`。 |
| `ForumIndexViewModel` | `LoginUserId, LoginUserName`（`forum.liquid` 初期描画用）。 |
| `ForumPostViewModel` | `PostId, UserId, UserName, Title, Body, CanEdit, Replies[]`。 |
| `ForumReplyViewModel` | `ReplyId, PostId, UserId, UserName, Body, CanEdit`。 |
| `ForumListResponseViewModel` | `IsSuccess, Message, Posts[], Page, PageCount, TotalCount`。 |
| `ForumActionResponseViewModel` | `IsSuccess, Message`（Create/Update/Delete/Reply 共通）。 |

### 4.4 Views (Liquid)
| View | 役割 | 渡される ViewModel |
|---|---|---|
| `_Shared/layout.liquid` | 共通 head/script/`#rootTemplate`。各画面の `page_body / component / onload` を差し込む。 | （継承時に渡される） |
| `index.liquid` | 「Hello」表示 + `vue-index` で `/get-message` `/update-message` をテスト。 | `IndexViewModel` |
| `login.liquid` | ログインフォーム。`InfoMessage`（登録完了等）と `ErrorMessage`。 | `AuthLoginViewModel` |
| `register.liquid` | 会員登録フォーム。`ErrorMessage`。 | `AuthRegisterViewModel` |
| `settings.liquid` | プロフィール/パスワード更新フォーム + 退会フォーム（パスワード必須・JS で confirm）。 | `AccountSettingsViewModel` |
| `forum.liquid` | Vue `vue-forum` を埋め込み、投稿/編集/削除/返信を fetch + Vue で実装。 | `ForumIndexViewModel` |

### 4.5 JS
* `scripts/vue_manager.js`
  * `class VueManager`
    * `init()` — Vue ルートアプリ生成、Vuetify を `use`、`mitt` イベント、デリミタを `${ }` に。
    * `mount()` — `#rootTemplate` にマウント。
    * `setComponent(name, content)` — 各画面の Vue コンポーネント登録。
    * `setPlugin / setValidatorDirective / checkInstallPlugin` — 拡張用。
* `forum.liquid` 内の `vue-forum` コンポーネントが `loadPosts / createPost / startEdit / cancelEdit / saveEdit / deletePost / submitReply / movePage` を持つ。
* `index.liquid` 内の `vue-index` は `getMessage / updateMessage`。

---

## 5. データベース接続 (Where SQL hits)

| テーブル | 触る場所 |
|---|---|
| `w2_User` | `AuthController`(SELECT/INSERT/EXISTS), `AccountController`(SELECT/UPDATE/SOFT DELETE) |
| `w2_ForumPost` | `ForumController`(SELECT list/count, INSERT, UPDATE 自分のみ, SOFT DELETE 自分のみ, EXISTS), `AccountController.SoftDeleteUserContent`(自分の投稿を一括 SOFT DELETE) |
| `w2_ForumReply` | `ForumController`(INSERT, SELECT IN), `AccountController.SoftDeleteUserContent`(自分の返信を一括 SOFT DELETE) |
| `w2_Sample` | `IndexController.GetMessage / UpdateMessage` (SqlKata) |

接続文字列は `w2.Common.Constants.STRING_SQL_CONNECTION` を全 Controller で共通利用。

論理削除は **`del_flg = 1`** で統一（物理削除は無い）。

---

## 6. 主要 SQL（コントローラ別）

### `AuthController`
* `SQL_SELECT_LOGIN_USER` — `WHERE login_id=@login_id AND password=@password AND del_flg=0`
* `SQL_SELECT_LOGIN_ID_COUNT` — `COUNT(1)` for unique check
* `SQL_INSERT_USER` — `(login_id, password, user_name, del_flg)`

### `AccountController`
* `SQL_SELECT_PROFILE` — `login_id, user_name FROM w2_User WHERE user_id=@user_id AND del_flg=0`
* `SQL_VERIFY_PASSWORD` — `COUNT(1)` で現在パスワード照合
* `SQL_UPDATE_USER_NAME` / `SQL_UPDATE_PASSWORD`
* `SQL_SOFT_DELETE_USER_POSTS` / `SQL_SOFT_DELETE_USER_REPLIES` / `SQL_SOFT_DELETE_USER` — 退会 (Transaction)

### `ForumController`
* `SQL_SELECT_POST_COUNT` / `SQL_SELECT_POST_LIST` — `OFFSET / FETCH NEXT` で 20 件ページング
* `SQL_INSERT_POST` / `SQL_UPDATE_OWN_POST` / `SQL_SOFT_DELETE_OWN_POST`
* `SQL_POST_EXISTS` / `SQL_INSERT_REPLY`
* 動的: `SELECT ... FROM w2_ForumReply WHERE post_id IN (@p0, @p1, ...)` を `AttachRepliesToPosts` 内で組立て。

---

## 7. 同じような関数 / コードの重複 (Similar method usage)

> リファクタリング候補。今は動作優先で意図的に重複している箇所もある。

### 7.1 `GetLoginUserId()` / `ClearLoginSession()` が **3 箇所**
| 場所 | コード |
|---|---|
| `AuthController.cs` (`ClearLoginSession`) | `Session.Remove(...)` |
| `AccountController.cs` (`GetLoginUserId`, `ClearLoginSession`) | 同じ実装 |
| `ForumController.cs` (`GetLoginUserId`) | 同じ実装 |

> セッションキー文字列 `"LoginUserId"` / `"LoginUserName"` も 3 ファイルで重複定義（`SESSION_KEY_LOGIN_USER_ID` 等）。
> 推奨: `Controller/Shared` に `LoginSessionHelper` を作って共通化。

### 7.2 `MESSAGE_LOGIN_REQUIRED = "ログインしてください。"` が `Forum`/`Account` 両方で定義
* `AuthController` には別の文言 `"ログインIDとパスワードを入力してください。"` があるので注意（同名定数で別意味）。

### 7.3 `BaseController` の `using (var connection ...) { ... using (var command ...) { ... } }` パターンが各 Controller で **大量に重複**
* `AuthController` 4 メソッド（DbCheck, TryLogin, ExistsActiveLoginId, InsertUser）
* `AccountController` 7 メソッド以上
* `ForumController` 7 メソッド以上
> 推奨: `SqlAccessor` 風のヘルパや、`w2.Common.Sql.SqlAccessor`（IndexController で利用）を共通利用すれば短くなる。

### 7.4 `AccountController` 内に **同じ SELECT を 3 メソッド** で再利用
* `LoadSettingsModel(userId)`
* `GetLoginIdFromDb(userId)`
* `GetUserNameFromDb(userId)`

すべて `SQL_SELECT_PROFILE` を使い `login_id` / `user_name` を取得。
> 推奨: `LoadSettingsModel` を呼んで結果から取り出すだけにすれば 2 メソッド分の重複 SQL アクセスを削れる。

### 7.5 `ForumController` の Update / Delete / CreateReply で **共通の "ログイン未満なら JSON 返す" 文** が同じ
```csharp
var loginUserId = this.GetLoginUserId();
if (loginUserId is null) { return JsonForJs(new ForumActionResponseViewModel{...}); }
```
> 推奨: ヘルパ化（例: `RequireLoginJson(out int userId)`）。

### 7.6 SQL 接続オープンの定型コード
すべて `using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION)) { connection.Open(); ... }`。
共通ヘルパで `Action<SqlConnection>` を渡す形にすれば縮められる。

### 7.7 Vue コンポーネント側 fetch ヘッダ・ボディ生成
`forum.liquid` 内の `createPost / saveEdit / deletePost / submitReply` は同じ
`'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8'` + `URLSearchParams` で書かれている。
> 推奨: `postForm(url, params)` ヘルパ関数で 1 本化可能。

### 7.8 `IndexController` の `UpdateMessage` と `UpdateMessage2`
同じ趣旨（メッセージ更新）でも、片方は SqlKata、もう片方は XML SQL の **サンプル比較用**。
画面では `UpdateMessage` のみ使用。

---

## 8. 機能別シーケンス図 (ASCII)

### 8.1 ログイン
```
login.liquid (form POST) ──► AuthController.Login(POST)
        │                          │
        │                          ├─ ValidateLogin(model)
        │                          ├─ TryLogin(model) ─► SQL_SELECT_LOGIN_USER ─► w2_User
        │                          │       │ OK → Session[LoginUserId/Name] 設定
        │                          ▼
        ◄──────────────── Redirect /forum   (失敗時は再表示)
```

### 8.2 投稿作成
```
forum.liquid (Vue createPost)
   │ fetch POST /api/forum/create  body: title, body
   ▼
ForumController.Create(title, body)
   ├─ GetLoginUserId()   (Session)
   ├─ Trim/Empty チェック
   ├─ InsertPost(loginUserId, title, body) ─► SQL_INSERT_POST ─► w2_ForumPost
   ▼
JsonForJs(IsSuccess=true, Message="投稿しました。")
   │
   ▼
Vue: this.message = ... ; this.loadPosts(1);
```

### 8.3 投稿編集 (自分の投稿のみ)
```
Vue saveEdit(postId) ─► fetch POST /api/forum/update
   ▼
ForumController.Update(postId, title, body)
   ├─ Validate
   ├─ UpdateOwnPost(loginUserId, postId, title, body)
   │       └─ SQL_UPDATE_OWN_POST  (WHERE post_id AND user_id AND del_flg=0)
   │       └─ ExecuteNonQuery() > 0  ⇒ 自分の投稿でない/存在しない場合 false
   ▼
IsSuccess=true / "この投稿は編集または削除できません。"
   ▼
Vue: cancelEdit() + loadPosts(this.page)
```

### 8.4 投稿削除 (論理削除)
```
Vue deletePost(postId) ─► confirm → fetch POST /api/forum/delete
   ▼
ForumController.Delete(postId)
   └─ DeleteOwnPost(loginUserId, postId)
        └─ SQL_SOFT_DELETE_OWN_POST  (del_flg = 1, 自分のみ)
   ▼
IsSuccess=true / FORBIDDEN
   ▼
Vue: loadPosts(this.page)  （一覧が空になっても同ページを再取得 → 0 件表示）
```

### 8.5 一覧取得 + 返信付与
```
Vue loadPosts(page) ─► fetch GET /api/forum/posts?page=N
   ▼
ForumController.Posts(page)
   ├─ GetPostCount() → SQL_SELECT_POST_COUNT
   ├─ GetPageCount(totalCount) → ceil(totalCount / 20)
   ├─ GetPostList(currentPage, loginUserId)
   │      └─ SQL_SELECT_POST_LIST (OFFSET/FETCH 20)  → w2_ForumPost INNER JOIN w2_User
   │      └─ CanEdit = (postUserId == loginUserId)
   ├─ AttachRepliesToPosts(posts, loginUserId)
   │      └─ 動的 IN 句 SQL → w2_ForumReply INNER JOIN w2_User
   │      └─ Dictionary<postId, List<Reply>> でグルーピング → 各 ForumPostViewModel.Replies に詰める
   ▼
JsonForJs(ForumListResponseViewModel)
   ▼
Vue: this.posts = result.Posts; replyDraft 初期化; pageCount 等更新
```

### 8.6 返信投稿
```
Vue submitReply(postId) ─► fetch POST /api/forum/reply
   ▼
ForumController.CreateReply(postId, body)
   ├─ Validate (空 body 不可)
   ├─ PostExists(postId) → SQL_POST_EXISTS
   ├─ InsertReply(postId, loginUserId, body) → SQL_INSERT_REPLY → w2_ForumReply
   ▼
IsSuccess=true / 該当投稿なし
   ▼
Vue: replyDraft[postId]=''; loadPosts(this.page);
```

### 8.7 アカウント設定 / 退会
```
GET /account/settings ─► AccountController.Settings()
        └─ LoadSettingsModel(userId) → SQL_SELECT_PROFILE
        └─ settings.liquid を返す

POST /account/update ─► AccountController.Update(model)
        ├─ ValidateLogin (Session)
        ├─ UserName 必須
        ├─ CurrentPassword 必須 → VerifyPassword(SQL_VERIFY_PASSWORD)
        ├─ UpdateUserName → SQL_UPDATE_USER_NAME ＋ Session[Name] 反映
        ├─ NewPassword + Confirm 一致時 → UpdatePassword (SQL_UPDATE_PASSWORD)
        └─ settings.liquid 再表示（SuccessMessage / ErrorMessage）

POST /account/delete ─► AccountController.Delete(model)
        ├─ DeletePassword 必須 → VerifyPassword
        ├─ Transaction:
        │    SQL_SOFT_DELETE_USER_POSTS    (w2_ForumPost: 自分の投稿)
        │    SQL_SOFT_DELETE_USER_REPLIES  (w2_ForumReply: 自分の返信)
        │    SQL_SOFT_DELETE_USER          (w2_User: 自分)
        ├─ ClearLoginSession()
        └─ Redirect /auth/login
```

---

## 9. 重要な命名規則 / ルール (現コード上の実態)

* **クラス名**: UpperCamelCase (`AuthController`, `AccountController`, `ForumController`)
* **メソッド名**: UpperCamelCase + 動詞開始 (`InsertPost`, `LoadSettingsModel`)
* **定数**: UPPER_SNAKE_CASE (`PAGE_SIZE`, `SQL_INSERT_POST`, `MESSAGE_LOGIN_REQUIRED`)
* **ローカル変数**: lowerCamelCase (`loginUserId`, `currentPage`)
* **SQL パラメータ**: snake_case + `@` (`@user_id`, `@post_id`)
* **DB カラム**: snake_case (`user_id`, `del_flg`)
* **論理削除統一**: `del_flg = 1`、SELECT は `WHERE del_flg = 0`
* **ページサイズ**: `private const int PAGE_SIZE = 20;` (ForumController) — W2 ルール準拠

---

## 10. 既知の小さな注意点 (TODO / Risk)

1. **セッション周り 3 箇所重複** → 共通ヘルパ未実装。将来 1 箇所に集約推奨。
2. **AccountController 内の 3 つのプロフィール SELECT** が同じ SQL 再利用。1 メソッド化可能。
3. **`AccountController.SoftDeleteUserContent`** は退会時の物理データ整合のためトランザクションを使用。`w2_ForumPost.user_id` の FK と整合。
4. **`forum.liquid` Vue は body を HTML エスケープせず描画** (`${ post.Body }`)。Vue は自動エスケープする (`{{ }}`相当の動作) ので XSS リスクは低いが、`v-html` を使ってないことを今後も維持。
5. **CSRF token (`AntiCsrfFormToken`)** は `BaseController.View` で `optionData` に渡しているが、Liquid 側で利用する箇所はサンプル中に未確認。必要なら埋め込みを追加。
6. **`IndexController.UpdateMessage / UpdateMessage2`** はサンプルなので、本番不要なら削除可。

---

## 11. 一覧 (ファイル ⇄ ルート ⇄ 主要メソッド)

| URL | Controller.Method | View / 戻り |
|-----|-----|-----|
| `GET /` | `IndexController.Index` | `index.liquid` |
| `GET /get-message` | `IndexController.GetMessage` | JSON |
| `POST /update-message` | `IndexController.UpdateMessage` | JSON |
| `POST /update-message2` | `IndexController.UpdateMessage2` | JSON |
| `GET /auth/login` | `AuthController.Login()` | `login.liquid` |
| `POST /auth/login` | `AuthController.Login(model)` | redirect or `login.liquid` |
| `GET /auth/register` | `AuthController.Register()` | `register.liquid` |
| `POST /auth/register` | `AuthController.Register(model)` | redirect or `register.liquid` |
| `POST /auth/logout` | `AuthController.Logout` | redirect `/auth/login` |
| `GET /auth/db-check` | `AuthController.DbCheck` | text/plain |
| `GET /forum` | `ForumController.Index` | `forum.liquid` |
| `GET /api/forum/posts` | `ForumController.Posts` | `ForumListResponseViewModel` JSON |
| `POST /api/forum/create` | `ForumController.Create` | `ForumActionResponseViewModel` JSON |
| `POST /api/forum/update` | `ForumController.Update` | JSON |
| `POST /api/forum/delete` | `ForumController.Delete` | JSON |
| `POST /api/forum/reply` | `ForumController.CreateReply` | JSON |
| `GET /account/settings` | `AccountController.Settings` | `settings.liquid` |
| `POST /account/update` | `AccountController.Update` | `settings.liquid` |
| `POST /account/delete` | `AccountController.Delete` | redirect `/auth/login` |

---

## 12. まとめ (One-paragraph summary)

ASP.NET MVC 5 の各 `*Controller` が `[Route]` 属性で URL を受け、ログイン状態は `Session` で管理。
HTML 画面は `BaseController.View()` から **Liquid テンプレート** で返し、動的 UI 部分は `forum.liquid` 内の **Vue コンポーネント (`vue-forum`)** が `fetch` で `/api/forum/...` を呼ぶ。
DB は ADO.NET (`SqlConnection` + `SqlCommand`) を直接使い、論理削除 `del_flg=1` で統一。
アカウント設定 / 退会は `AccountController`、認証は `AuthController`、掲示板 (一覧/作成/編集/削除/返信) は `ForumController`、トップ + サンプル更新は `IndexController` がそれぞれ担当。
共通インフラ (DI, ルーティング, Liquid レンダリング, JSON, セッションヘルパ) は `Global.asax`, `App_Start/*`, `Controller/Shared/BaseController.cs`, `scripts/vue_manager.js` に集約。
