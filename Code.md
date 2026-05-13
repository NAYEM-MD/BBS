# 1. Project Scan Summary

This repository is a .NET Framework 4.8 bulletin board assignment with two web applications inside one solution:

- `Web/w2.BBS.Front`: public-facing site built with ASP.NET MVC 5, Liquid views, and Vue + `fetch`
- `Web/w2.BBS.Manager`: admin site built with ASP.NET Web Forms

Current implementation status:

- The front site is partially implemented.
- Login, register, forum list, forum create, and 20-item paging already exist in some form.
- The admin site is not implemented yet. It is still a `Hello, World!` placeholder.
- The shared domain projects are mostly empty, but this is acceptable for submission because `README.md` explicitly says direct implementation in the web projects is allowed.

Completed features already present:

- MVC 5 front project exists and runs with Liquid rendering infrastructure.
- Vue manager script exists.
- Login page exists.
- Register page exists.
- Forum page exists.
- Forum API endpoints for list/create exist.
- Front paging constant is already `20`.

Incomplete or missing features:

- Front root page is still demo/sample code instead of assignment behavior.
- Manager side is still placeholder-only.
- No real admin list/delete workflow exists.
- No practical final completion flow exists from root page to user pages and manager pages.
- Database seed/template files required by the README are missing from the repository tree.

Broken or incorrect parts found:

- `Web/w2.BBS.Front/App_Start/RouteConfig.cs` points unmatched requests to a non-existent `ErrorController`.
- `Web/w2.BBS.Front/Controller/IndexController.cs` is still sample/demo code.
- `Web/w2.BBS.Front/Views/index.liquid` contains JavaScript errors (`res.status` is used, but `res` does not exist).
- `Web/w2.BBS.Front/Xml/Db/Sample.xml` updates `message_test` instead of `message_text`.
- `Web/w2.BBS.Front/Views/login.liquid`, `register.liquid`, and `forum.liquid` hard-code `/Training.BBS/Web/w2.BBS.Front/...` instead of using `RootUrl`.
- `Web/w2.BBS.Manager/Global.asax.cs` sets `Constants.APPLICATION_NAME` to `w2.BBS.Front` instead of `w2.BBS.Manager`.
- `Web/w2.BBS.Manager/Default.aspx` and `Default.aspx.cs` are only placeholder content.
- `Web/w2.BBS.Front/w2.BBS.Front.csproj` contains duplicate compile includes for some view model files.

Rule violations or risky parts:

- Razor must not be used; this project correctly uses Liquid, but the final manual implementation must continue that rule.
- Front JavaScript must stay simple and use Vue + `fetch`; do not replace it with jQuery AJAX or another framework.
- Admin must remain Web Forms; do not turn manager pages into MVC pages.
- Hard-coded front URLs violate the project’s root-path-friendly style and are risky under IIS virtual directories.
- The current login/register code stores raw passwords. That is risky in real systems, but changing the schema and auth strategy here would over-expand the assignment. Keep it practical unless the external assignment document explicitly demands hashing.

Important caution:

- `README.md` says `Web/w2.BBS.Front/App_Data/bbs.mdf.template` and `bbs_log.ldf.template` should exist and be renamed. They are not present in this repository snapshot. This is a blocker you must resolve first, either by restoring the original template files from the assignment package or by manually creating the database with equivalent tables.

# 2. Company Rules Summary Used For This Project

Only the rules that directly affect this project are summarized below.

## Environment and setup rules

- Use Visual Studio 2022.
- Enable IIS on Windows.
- Set `ASP.NET State Service` startup type to `Automatic`.
- Place the project under `C:\inetpub\wwwroot\Training.BBS` as the working path.
- Run Visual Studio as Administrator.
- If SQL access fails, the README says to change the IIS application pool identity to `LocalSystem`.
- The front project expects `App_Data/bbs.mdf` and `App_Data/bbs_log.ldf`.

## Architecture rules

- Front side must stay ASP.NET MVC 5.
- Front views must use Liquid, not Razor.
- Front interactive behavior should use Vue + `fetch`.
- Manager side must stay ASP.NET Web Forms.
- Commons domain projects can be left unused for submission because the README explicitly permits writing logic directly in the web project.

## UI and paging rules

- Forum list paging should use 20 items per page.
- Front and admin responsibilities must stay separated.
- Front is for general users.
- Manager is for administrators.

## Naming and file rules

- Controllers should stay in `Controller` with PascalCase controller class names.
- View models should stay in `ViewModels` with PascalCase names ending in `ViewModel`.
- Liquid view file names should stay simple and aligned to the screen purpose, such as `login.liquid`, `register.liquid`, `forum.liquid`.
- Keep file and symbol names practical and consistent with the existing project.

## Comment rules

- Keep comments short and only where they explain real intent.
- Do not add noisy comments for obvious lines.
- Existing project style uses Japanese XML summary comments in C# files. Follow that style.

## JavaScript rules

- Use Vue 3 already loaded in `layout.liquid`.
- Use `fetch`, not jQuery AJAX.
- Keep the code simple and page-local.
- Do not hard-code the IIS virtual directory path; use `window.apiRoot` or `RootUrl`.

## SQL rules

- Use parameterized SQL only.
- Do not build SQL with string concatenation from user input.
- Continue using `del_flg` soft-delete style because the current schema and code clearly follow that pattern.

# 3. Final Development Order

Use this implementation order from the current state:

1. Restore or create the database files required by the front project.
2. Fix the front project structure problems first:
   - remove sample/demo entry behavior
   - remove broken fallback routing
   - remove hard-coded front URLs
   - remove obviously broken sample artifacts from active flow
3. Finalize authentication flow:
   - login
   - register
   - logout
   - root page redirect
4. Finalize forum flow:
   - forum page
   - post list
   - create post
   - 20-item paging
5. Implement the Web Forms manager dashboard:
   - user list
   - post list
   - 20-item paging for both sections
   - soft delete actions
6. Fix remaining project metadata issues:
   - manager application name
   - duplicate compile include lines
7. Run full manual verification in IIS for both front and manager.

This order is safest because the front side already contains partial work, while the manager side is still placeholder-only.

# 4. Step-by-Step Manual Implementation Guide

## Step 1 - Restore the Database Prerequisite

### Goal

Make sure the front project can actually connect to a database before you continue implementation.

### Why this step comes now

Almost every front and manager feature depends on `w2_User` and `w2_ForumPost`. Without the database, all later steps are blocked.

### Files to create manually

- If the original assignment package still has them:
  - `Web/w2.BBS.Front/App_Data/bbs.mdf`
  - `Web/w2.BBS.Front/App_Data/bbs_log.ldf`
- If the original template files are missing and you must recreate the schema manually:
  - create the `App_Data` folder
  - attach or create a SQL Server database that matches the connection expected by `w2.Common.Constants.STRING_SQL_CONNECTION`

### Files to edit manually

- None in source code for this step.

### Exact file locations

- `Web/w2.BBS.Front/App_Data/`

### Whether to replace full file or only add code

- Not a source-code edit.

### Exact SQL you should execute manually if you must recreate the tables

Run the following SQL in the database that the project uses:

```sql
CREATE TABLE w2_User
(
  user_id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  login_id NVARCHAR(50) NOT NULL,
  password NVARCHAR(100) NOT NULL,
  user_name NVARCHAR(100) NOT NULL,
  mail_address NVARCHAR(200) NOT NULL,
  date_created DATETIME NOT NULL,
  date_changed DATETIME NOT NULL,
  del_flg BIT NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX IX_w2_User_login_id
ON w2_User(login_id)
WHERE del_flg = 0;

CREATE TABLE w2_ForumPost
(
  post_id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  user_id INT NOT NULL,
  title NVARCHAR(100) NOT NULL,
  body NVARCHAR(MAX) NOT NULL,
  date_created DATETIME NOT NULL,
  date_changed DATETIME NOT NULL,
  del_flg BIT NOT NULL DEFAULT 0
);

ALTER TABLE w2_ForumPost
ADD CONSTRAINT FK_w2_ForumPost_w2_User
FOREIGN KEY (user_id) REFERENCES w2_User(user_id);

CREATE INDEX IX_w2_ForumPost_List
ON w2_ForumPost(del_flg, date_created DESC, post_id DESC);
```

### What this code does

- Creates the member table used by login/register.
- Creates the forum post table used by the front forum and manager.
- Keeps `del_flg` soft-delete behavior consistent with the existing implementation.

### Company rules followed

- Practical schema only.
- Simple SQL.
- Supports current MVC + Web Forms behavior without redesign.

### Exact test steps after finishing this step

1. Confirm `App_Data` exists.
2. Confirm the front project can open SQL connections.
3. If you want a quick check, temporarily browse an auth page after later steps are done.

### Expected result

The project has a usable database with `w2_User` and `w2_ForumPost`.

### Common mistakes to avoid

- Do not create different table names.
- Do not omit `del_flg`.
- Do not skip `date_created` and `date_changed`.
- Do not use a different identity key name than the current code expects.

## Step 2 - Fix Front Routing and Remove Demo Entry Logic

### Goal

Replace the sample top page flow with assignment flow and remove the broken catch-all route.

### Why this step comes now

The current root page is demo code and the current fallback route points to a missing controller. That makes the application unstable even before feature testing.

### Files to edit manually

- `Web/w2.BBS.Front/App_Start/RouteConfig.cs`
- `Web/w2.BBS.Front/Controller/IndexController.cs`
- `Web/w2.BBS.Front/w2.BBS.Front.csproj`

### Exact file locations

- `Web/w2.BBS.Front/App_Start/RouteConfig.cs`
- `Web/w2.BBS.Front/Controller/IndexController.cs`
- `Web/w2.BBS.Front/w2.BBS.Front.csproj`

### Whether to replace the full file or only add code

- Replace full file for `RouteConfig.cs`
- Replace full file for `IndexController.cs`
- Partial edit for `w2.BBS.Front.csproj`

### Exact full code block to type for `Web/w2.BBS.Front/App_Start/RouteConfig.cs`

```csharp
// (c) 2025 W2 Co.,Ltd.

using System.Web.Mvc;
using System.Web.Routing;

namespace w2.BBS.Front
{
	/// <summary>
	/// ルーティング設定
	/// </summary>
	public class RouteConfig
	{
		/// <summary>
		/// ルーティング登録
		/// </summary>
		/// <param name="routes">ルートコレクション</param>
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.RouteExistingFiles = false;

			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("Scripts/{*pathInfo}");
			routes.IgnoreRoute("{resource}.ashx");
			routes.IgnoreRoute("Theme/{*pathInfo}");

			routes.MapMvcAttributeRoutes();

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new
				{
					controller = "Index",
					action = "Index",
					id = UrlParameter.Optional,
				});
		}
	}
}
```

### Exact full code block to type for `Web/w2.BBS.Front/Controller/IndexController.cs`

```csharp
// (c) 2025 W2 Co.,Ltd.

using System.Web.Mvc;
using w2.BBS.Front.Controller.Shared;

namespace w2.BBS.Front.Controller
{
	/// <summary>
	/// トップページ
	/// </summary>
	public class IndexController : BaseController
	{
		/// <summary>
		/// ルートアクセス
		/// </summary>
		/// <returns>ログイン画面へリダイレクト</returns>
		[HttpGet]
		[Route("~/")]
		public ActionResult Index()
		{
			return this.Redirect("~/auth/login");
		}
	}
}
```

### Partial edit for `Web/w2.BBS.Front/w2.BBS.Front.csproj`

In the `<ItemGroup>` that contains the `Compile Include=` entries, remove the duplicate lines at the end:

```xml
<Compile Include="ViewModels\ForumPostViewModel.cs" />
<Compile Include="ViewModels\ForumListResponseViewModel.cs" />
<Compile Include="ViewModels\ForumActionResponseViewModel.cs" />
```

Keep only one include per file.

### What this code does

- Keeps MVC 5 routing valid.
- Makes the root page go to the login flow instead of the demo/sample page.
- Removes the broken dependency on a non-existent error controller.
- Prevents duplicate compile item confusion in the project file.

### Company rules followed

- MVC 5 remains intact.
- No Razor is introduced.
- Naming stays aligned to existing project style.

### Exact test steps after finishing this step

1. Browse the front root URL.
2. Confirm it redirects to `/auth/login`.
3. Browse an unknown URL under the front site and confirm you no longer hit the missing `ErrorController` problem.

### Expected result

The front site enters the assignment flow cleanly instead of showing demo/sample behavior.

### Common mistakes to avoid

- Do not keep the old `CatchAll` route.
- Do not leave the sample DB actions in `IndexController`.
- Do not accidentally remove the attribute route registration.

## Step 3 - Finalize the Authentication Flow

### Goal

Keep login/register/logout practical, stable, and path-safe under IIS.

### Why this step comes now

Forum access depends on a working login flow. The current controller is mostly usable, but the views still hard-code front URLs and the controller still contains a debug endpoint that should not remain in the final assignment flow.

### Files to edit manually

- `Web/w2.BBS.Front/Controller/AuthController.cs`
- `Web/w2.BBS.Front/Views/login.liquid`
- `Web/w2.BBS.Front/Views/register.liquid`

### Exact file locations

- `Web/w2.BBS.Front/Controller/AuthController.cs`
- `Web/w2.BBS.Front/Views/login.liquid`
- `Web/w2.BBS.Front/Views/register.liquid`

### Whether to replace the full file or only add code

- Replace full file for all three

### Exact full code block to type for `Web/w2.BBS.Front/Controller/AuthController.cs`

```csharp
// (c) 2025 W2 Co.,Ltd.

using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using w2.BBS.Front.Controller.Shared;
using w2.BBS.Front.ViewModels;
using w2.Common;

namespace w2.BBS.Front.Controller
{
	/// <summary>
	/// 認証
	/// </summary>
	public class AuthController : BaseController
	{
		private const string VIEW_LOGIN = "login.liquid";
		private const string VIEW_REGISTER = "register.liquid";

		private const string SESSION_KEY_LOGIN_USER_ID = "LoginUserId";
		private const string SESSION_KEY_LOGIN_USER_NAME = "LoginUserName";

		private const string QUERY_KEY_REGISTERED = "registered";

		private const string MESSAGE_REGISTER_COMPLETED = "会員登録が完了しました。ログインしてください。";
		private const string MESSAGE_LOGIN_REQUIRED = "ログインIDとパスワードを入力してください。";
		private const string MESSAGE_LOGIN_FAILED = "ログインIDまたはパスワードが正しくありません。";
		private const string MESSAGE_REGISTER_REQUIRED = "すべて入力してください。";
		private const string MESSAGE_LOGIN_ID_DUPLICATE = "そのログインIDは既に使われています。";

		private const string SQL_SELECT_LOGIN_USER =
@"SELECT TOP 1
	user_id,
	user_name
FROM w2_User
WHERE login_id = @login_id
AND password = @password
AND del_flg = 0";

		private const string SQL_SELECT_LOGIN_ID_COUNT =
@"SELECT COUNT(1)
FROM w2_User
WHERE login_id = @login_id
AND del_flg = 0";

		private const string SQL_INSERT_USER =
@"INSERT INTO w2_User
(
	login_id,
	password,
	user_name,
	mail_address,
	date_created,
	date_changed,
	del_flg
)
VALUES
(
	@login_id,
	@password,
	@user_name,
	@mail_address,
	@date_created,
	@date_changed,
	0
)";

		/// <summary>
		/// ログイン画面
		/// </summary>
		[HttpGet]
		[Route("~/auth/login")]
		public ActionResult Login()
		{
			if (this.IsLoggedIn())
			{
				return this.Redirect("~/forum");
			}

			var model = new AuthLoginViewModel();
			this.SetLoginInfoMessage(model);

			return base.View(VIEW_LOGIN, model);
		}

		/// <summary>
		/// ログイン
		/// </summary>
		/// <param name="model">ログイン画面モデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[Route("~/auth/login")]
		public ActionResult Login(AuthLoginViewModel model)
		{
			var loginModel = model ?? new AuthLoginViewModel();

			if (this.ValidateLogin(loginModel) == false)
			{
				return base.View(VIEW_LOGIN, loginModel);
			}

			if (this.TryLogin(loginModel) == false)
			{
				loginModel.ErrorMessage = MESSAGE_LOGIN_FAILED;
				return base.View(VIEW_LOGIN, loginModel);
			}

			return this.Redirect("~/forum");
		}

		/// <summary>
		/// 会員登録画面
		/// </summary>
		[HttpGet]
		[Route("~/auth/register")]
		public ActionResult Register()
		{
			if (this.IsLoggedIn())
			{
				return this.Redirect("~/forum");
			}

			return base.View(VIEW_REGISTER, new AuthRegisterViewModel());
		}

		/// <summary>
		/// 会員登録
		/// </summary>
		/// <param name="model">会員登録画面モデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[Route("~/auth/register")]
		public ActionResult Register(AuthRegisterViewModel model)
		{
			var registerModel = model ?? new AuthRegisterViewModel();

			if (this.ValidateRegister(registerModel) == false)
			{
				return base.View(VIEW_REGISTER, registerModel);
			}

			if (this.ExistsActiveLoginId(registerModel.LoginId))
			{
				registerModel.ErrorMessage = MESSAGE_LOGIN_ID_DUPLICATE;
				return base.View(VIEW_REGISTER, registerModel);
			}

			this.InsertUser(registerModel);
			return this.Redirect("~/auth/login?registered=1");
		}

		/// <summary>
		/// ログアウト
		/// </summary>
		[HttpPost]
		[Route("~/auth/logout")]
		public ActionResult Logout()
		{
			this.ClearLoginSession();
			return this.Redirect("~/auth/login");
		}

		/// <summary>
		/// ログイン済みか
		/// </summary>
		/// <returns>結果</returns>
		private bool IsLoggedIn()
		{
			return this.Session[SESSION_KEY_LOGIN_USER_ID] != null;
		}

		/// <summary>
		/// ログイン完了メッセージ設定
		/// </summary>
		/// <param name="model">ログイン画面モデル</param>
		private void SetLoginInfoMessage(AuthLoginViewModel model)
		{
			if (model == null)
			{
				return;
			}

			var isRegistered = (this.Request.QueryString[QUERY_KEY_REGISTERED] == "1");
			if (isRegistered)
			{
				model.InfoMessage = MESSAGE_REGISTER_COMPLETED;
			}
		}

		/// <summary>
		/// ログイン入力チェック
		/// </summary>
		/// <param name="model">ログイン画面モデル</param>
		/// <returns>結果</returns>
		private bool ValidateLogin(AuthLoginViewModel model)
		{
			if (model == null)
			{
				return false;
			}

			var hasLoginId = string.IsNullOrWhiteSpace(model.LoginId) == false;
			var hasPassword = string.IsNullOrWhiteSpace(model.Password) == false;
			if (hasLoginId && hasPassword)
			{
				return true;
			}

			model.ErrorMessage = MESSAGE_LOGIN_REQUIRED;
			return false;
		}

		/// <summary>
		/// 会員登録入力チェック
		/// </summary>
		/// <param name="model">会員登録画面モデル</param>
		/// <returns>結果</returns>
		private bool ValidateRegister(AuthRegisterViewModel model)
		{
			if (model == null)
			{
				return false;
			}

			var hasLoginId = string.IsNullOrWhiteSpace(model.LoginId) == false;
			var hasPassword = string.IsNullOrWhiteSpace(model.Password) == false;
			var hasUserName = string.IsNullOrWhiteSpace(model.UserName) == false;
			var hasMailAddress = string.IsNullOrWhiteSpace(model.MailAddress) == false;

			if (hasLoginId
				&& hasPassword
				&& hasUserName
				&& hasMailAddress)
			{
				return true;
			}

			model.ErrorMessage = MESSAGE_REGISTER_REQUIRED;
			return false;
		}

		/// <summary>
		/// ログイン処理
		/// </summary>
		/// <param name="model">ログイン画面モデル</param>
		/// <returns>結果</returns>
		private bool TryLogin(AuthLoginViewModel model)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_LOGIN_USER, connection))
				{
					command.Parameters.AddWithValue("@login_id", model.LoginId.Trim());
					command.Parameters.AddWithValue("@password", model.Password.Trim());

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read() == false)
						{
							return false;
						}

						this.Session[SESSION_KEY_LOGIN_USER_ID] = Convert.ToInt32(reader["user_id"]);
						this.Session[SESSION_KEY_LOGIN_USER_NAME] = Convert.ToString(reader["user_name"]);
						return true;
					}
				}
			}
		}

		/// <summary>
		/// ログインID存在確認
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <returns>結果</returns>
		private bool ExistsActiveLoginId(string loginId)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_LOGIN_ID_COUNT, connection))
				{
					command.Parameters.AddWithValue("@login_id", loginId.Trim());
					var count = Convert.ToInt32(command.ExecuteScalar());
					return (count > 0);
				}
			}
		}

		/// <summary>
		/// 会員登録
		/// </summary>
		/// <param name="model">会員登録画面モデル</param>
		private void InsertUser(AuthRegisterViewModel model)
		{
			var currentDateTime = DateTime.Now;

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_INSERT_USER, connection))
				{
					command.Parameters.AddWithValue("@login_id", model.LoginId.Trim());
					command.Parameters.AddWithValue("@password", model.Password.Trim());
					command.Parameters.AddWithValue("@user_name", model.UserName.Trim());
					command.Parameters.AddWithValue("@mail_address", model.MailAddress.Trim());
					command.Parameters.AddWithValue("@date_created", currentDateTime);
					command.Parameters.AddWithValue("@date_changed", currentDateTime);
					command.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// ログインセッションクリア
		/// </summary>
		private void ClearLoginSession()
		{
			this.Session.Remove(SESSION_KEY_LOGIN_USER_ID);
			this.Session.Remove(SESSION_KEY_LOGIN_USER_NAME);
		}
	}
}
```

### Exact full code block to type for `Web/w2.BBS.Front/Views/login.liquid`

```liquid
{% _layout 'layout.liquid' %}

{% _area title %}ログイン{% end_area %}

{% _area head_resource %}
{% end_area %}

{% _area page_body %}
<div>
  <h1>ログイン</h1>

  {% if InfoMessage != blank %}
  <p>{{ InfoMessage }}</p>
  {% endif %}

  {% if ErrorMessage != blank %}
  <p>{{ ErrorMessage }}</p>
  {% endif %}

  <form method="post" action="{{ RootUrl | append: "/auth/login" }}">
    <div>
      <label for="loginId">ログインID</label>
      <input type="text" id="loginId" name="LoginId" value="{{ LoginId }}" />
    </div>

    <div>
      <label for="password">パスワード</label>
      <input type="password" id="password" name="Password" />
    </div>

    <div>
      <button type="submit">ログイン</button>
    </div>
  </form>

  <div>
    <a href="{{ RootUrl | append: "/auth/register" }}">会員登録はこちら</a>
  </div>
</div>
{% end_area %}

{% _area component %}
{% end_area %}
```

### Exact full code block to type for `Web/w2.BBS.Front/Views/register.liquid`

```liquid
{% _layout 'layout.liquid' %}

{% _area title %}会員登録{% end_area %}

{% _area head_resource %}
{% end_area %}

{% _area page_body %}
<div>
  <h1>会員登録</h1>

  {% if ErrorMessage != blank %}
  <p>{{ ErrorMessage }}</p>
  {% endif %}

  <form method="post" action="{{ RootUrl | append: "/auth/register" }}">
    <div>
      <label for="loginId">ログインID</label>
      <input type="text" id="loginId" name="LoginId" value="{{ LoginId }}" />
    </div>

    <div>
      <label for="userName">ユーザー名</label>
      <input type="text" id="userName" name="UserName" value="{{ UserName }}" />
    </div>

    <div>
      <label for="mailAddress">メールアドレス</label>
      <input type="text" id="mailAddress" name="MailAddress" value="{{ MailAddress }}" />
    </div>

    <div>
      <label for="password">パスワード</label>
      <input type="password" id="password" name="Password" />
    </div>

    <div>
      <button type="submit">登録する</button>
    </div>
  </form>

  <div>
    <a href="{{ RootUrl | append: "/auth/login" }}">ログイン画面へ戻る</a>
  </div>
</div>
{% end_area %}

{% _area component %}
{% end_area %}
```

### What this code does

- Removes the debug DB-check action from the final flow.
- Keeps auth logic simple and aligned to the current project.
- Fixes path handling by using `RootUrl`.
- Stops echoing the password back into the HTML.

### Company rules followed

- MVC 5 + Liquid kept.
- Practical implementation inside the web project.
- Simple, direct form posting.

### Exact test steps after finishing this step

1. Open `/auth/register`.
2. Register a new user.
3. Confirm redirect to `/auth/login?registered=1`.
4. Log in with the new account.
5. Confirm redirect to `/forum`.
6. Click logout after Step 4 is finished and confirm redirect back to login.

### Expected result

The user can register, log in, and log out without broken paths.

### Common mistakes to avoid

- Do not keep hard-coded `/Training.BBS/Web/w2.BBS.Front/...` links.
- Do not keep the password value binding in the view.
- Do not keep the debug DB-check endpoint in the final assignment flow.

## Step 4 - Finalize the Forum Page and Vue + Fetch Flow

### Goal

Keep the forum page consistent with project rules and make the front forum stable under IIS virtual paths.

### Why this step comes now

The forum controller is already close to usable. The main remaining work is view cleanup and path correction.

### Files to edit manually

- `Web/w2.BBS.Front/Controller/ForumController.cs`
- `Web/w2.BBS.Front/Views/forum.liquid`

### Exact file locations

- `Web/w2.BBS.Front/Controller/ForumController.cs`
- `Web/w2.BBS.Front/Views/forum.liquid`

### Whether to replace the full file or only add code

- Replace full file for both

### Exact full code block to type for `Web/w2.BBS.Front/Controller/ForumController.cs`

```csharp
// (c) 2025 W2 Co.,Ltd.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using w2.BBS.Front.Controller.Shared;
using w2.BBS.Front.ViewModels;
using w2.Common;

namespace w2.BBS.Front.Controller
{
	/// <summary>
	/// 掲示板
	/// </summary>
	public class ForumController : BaseController
	{
		private const string VIEW_FORUM = "forum.liquid";

		private const string SESSION_KEY_LOGIN_USER_ID = "LoginUserId";
		private const string SESSION_KEY_LOGIN_USER_NAME = "LoginUserName";

		private const int PAGE_SIZE = 20;

		private const string MESSAGE_LOGIN_REQUIRED = "ログインしてください。";
		private const string MESSAGE_POST_INPUT_REQUIRED = "タイトルと本文を入力してください。";
		private const string MESSAGE_POST_CREATED = "投稿しました。";

		private const string SQL_SELECT_POST_COUNT =
@"SELECT COUNT(1)
FROM w2_ForumPost
WHERE del_flg = 0";

		private const string SQL_SELECT_POST_LIST =
@"SELECT
	p.post_id,
	p.user_id,
	u.user_name,
	p.title,
	p.body,
	p.date_created
FROM w2_ForumPost p
INNER JOIN w2_User u ON
(
	p.user_id = u.user_id
	AND u.del_flg = 0
)
WHERE p.del_flg = 0
ORDER BY
	p.date_created DESC,
	p.post_id DESC
OFFSET @offset ROWS
FETCH NEXT @page_size ROWS ONLY";

		private const string SQL_INSERT_POST =
@"INSERT INTO w2_ForumPost
(
	user_id,
	title,
	body,
	date_created,
	date_changed,
	del_flg
)
VALUES
(
	@user_id,
	@title,
	@body,
	@date_created,
	@date_changed,
	0
)";

		/// <summary>
		/// 掲示板トップ
		/// </summary>
		[HttpGet]
		[Route("~/forum")]
		public ActionResult Index()
		{
			var loginUserId = this.GetLoginUserId();
			if (loginUserId == null)
			{
				return this.Redirect("~/auth/login");
			}

			var model = new ForumIndexViewModel
			{
				LoginUserId = loginUserId.Value,
				LoginUserName = Convert.ToString(this.Session[SESSION_KEY_LOGIN_USER_NAME]),
			};

			return base.View(VIEW_FORUM, model);
		}

		/// <summary>
		/// 投稿一覧取得
		/// </summary>
		/// <param name="page">ページ番号</param>
		/// <returns>JSON結果</returns>
		[HttpGet]
		[Route("~/api/forum/posts")]
		public ActionResult Posts(int page = 1)
		{
			var loginUserId = this.GetLoginUserId();
			if (loginUserId == null)
			{
				return this.JsonForJs(new ForumListResponseViewModel
				{
					IsSuccess = false,
					Message = MESSAGE_LOGIN_REQUIRED,
				});
			}

			var currentPage = (page < 1) ? 1 : page;
			var totalCount = this.GetPostCount();
			var pageCount = this.GetPageCount(totalCount);

			if (currentPage > pageCount)
			{
				currentPage = pageCount;
			}

			var posts = this.GetPostList(currentPage, loginUserId.Value);

			return this.JsonForJs(new ForumListResponseViewModel
			{
				IsSuccess = true,
				Posts = posts,
				Page = currentPage,
				PageCount = pageCount,
				TotalCount = totalCount,
			});
		}

		/// <summary>
		/// 投稿作成
		/// </summary>
		/// <param name="title">タイトル</param>
		/// <param name="body">本文</param>
		/// <returns>JSON結果</returns>
		[HttpPost]
		[Route("~/api/forum/create")]
		public ActionResult Create(string title, string body)
		{
			var loginUserId = this.GetLoginUserId();
			if (loginUserId == null)
			{
				return this.JsonForJs(new ForumActionResponseViewModel
				{
					IsSuccess = false,
					Message = MESSAGE_LOGIN_REQUIRED,
				});
			}

			var inputTitle = (title ?? string.Empty).Trim();
			var inputBody = (body ?? string.Empty).Trim();

			if (string.IsNullOrEmpty(inputTitle)
				|| string.IsNullOrEmpty(inputBody))
			{
				return this.JsonForJs(new ForumActionResponseViewModel
				{
					IsSuccess = false,
					Message = MESSAGE_POST_INPUT_REQUIRED,
				});
			}

			this.InsertPost(loginUserId.Value, inputTitle, inputBody);

			return this.JsonForJs(new ForumActionResponseViewModel
			{
				IsSuccess = true,
				Message = MESSAGE_POST_CREATED,
			});
		}

		/// <summary>
		/// ログイン会員ID取得
		/// </summary>
		/// <returns>会員ID</returns>
		private int? GetLoginUserId()
		{
			var loginUserId = this.Session[SESSION_KEY_LOGIN_USER_ID];
			if (loginUserId == null)
			{
				return null;
			}

			return Convert.ToInt32(loginUserId);
		}

		/// <summary>
		/// 投稿件数取得
		/// </summary>
		/// <returns>件数</returns>
		private int GetPostCount()
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_POST_COUNT, connection))
				{
					return Convert.ToInt32(command.ExecuteScalar());
				}
			}
		}

		/// <summary>
		/// 投稿一覧取得
		/// </summary>
		/// <param name="page">ページ番号</param>
		/// <param name="loginUserId">ログイン会員ID</param>
		/// <returns>投稿一覧</returns>
		private ForumPostViewModel[] GetPostList(int page, int loginUserId)
		{
			var postList = new List<ForumPostViewModel>();
			var offset = (page - 1) * PAGE_SIZE;

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_POST_LIST, connection))
				{
					command.Parameters.AddWithValue("@offset", offset);
					command.Parameters.AddWithValue("@page_size", PAGE_SIZE);

					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							var postUserId = Convert.ToInt32(reader["user_id"]);

							postList.Add(new ForumPostViewModel
							{
								PostId = Convert.ToInt32(reader["post_id"]),
								UserId = postUserId,
								UserName = Convert.ToString(reader["user_name"]),
								Title = Convert.ToString(reader["title"]),
								Body = Convert.ToString(reader["body"]),
								DateCreatedText = Convert.ToDateTime(reader["date_created"]).ToString("yyyy/MM/dd HH:mm"),
								CanEdit = (postUserId == loginUserId),
							});
						}
					}
				}
			}

			return postList.ToArray();
		}

		/// <summary>
		/// 投稿登録
		/// </summary>
		/// <param name="loginUserId">会員ID</param>
		/// <param name="title">タイトル</param>
		/// <param name="body">本文</param>
		private void InsertPost(int loginUserId, string title, string body)
		{
			var currentDateTime = DateTime.Now;

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_INSERT_POST, connection))
				{
					command.Parameters.AddWithValue("@user_id", loginUserId);
					command.Parameters.AddWithValue("@title", title);
					command.Parameters.AddWithValue("@body", body);
					command.Parameters.AddWithValue("@date_created", currentDateTime);
					command.Parameters.AddWithValue("@date_changed", currentDateTime);
					command.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// 総ページ数取得
		/// </summary>
		/// <param name="totalCount">総件数</param>
		/// <returns>ページ数</returns>
		private int GetPageCount(int totalCount)
		{
			if (totalCount == 0)
			{
				return 1;
			}

			return (int)Math.Ceiling((decimal)totalCount / PAGE_SIZE);
		}
	}
}
```

### Exact full code block to type for `Web/w2.BBS.Front/Views/forum.liquid`

```liquid
{% _layout 'layout.liquid' %}

{% _area title %}掲示板{% end_area %}

{% _area head_resource %}
{% end_area %}

{% _area page_body %}
<div>
  <h1>掲示板</h1>

  <p>ログインしました。</p>
  <p>ユーザー名: {{ LoginUserName }}</p>

  <form method="post" action="{{ RootUrl | append: "/auth/logout" }}">
    <button type="submit">ログアウト</button>
  </form>

  <hr />

  <vue-forum></vue-forum>
</div>
{% end_area %}

{% _area component %}
<script type="text/x-template" id="vue-forum-template">
  <div>
    <h2>新規投稿</h2>

    <div>
      <p v-if="message">${ message }</p>
      <p v-if="errorMessage">${ errorMessage }</p>
    </div>

    <div>
      <label for="postTitle">タイトル</label>
      <input type="text" id="postTitle" v-model="newTitle" />
    </div>

    <div>
      <label for="postBody">本文</label>
      <textarea id="postBody" v-model="newBody"></textarea>
    </div>

    <div>
      <button @click="createPost" :disabled="isSaving">投稿する</button>
    </div>

    <hr />

    <h2>投稿一覧</h2>

    <p v-if="hasPosts === false">投稿はまだありません。</p>

    <div v-for="post in posts" :key="post.PostId">
      <h3>${ post.Title }</h3>
      <p>投稿者: ${ post.UserName }</p>
      <p>投稿日: ${ post.DateCreatedText }</p>
      <p>${ post.Body }</p>
      <hr />
    </div>

    <div>
      <button @click="movePage(page - 1)" :disabled="canPrevPage === false">前へ</button>
      <span>${ pageLabel }</span>
      <button @click="movePage(page + 1)" :disabled="canNextPage === false">次へ</button>
    </div>
  </div>
</script>

<script>
  {% _javascript onload %}
  g_vueManager.setComponent('vue-forum', {
    template: '#vue-forum-template',
    data() {
      return {
        posts: [],
        page: 1,
        pageCount: 1,
        totalCount: 0,
        newTitle: '',
        newBody: '',
        message: '',
        errorMessage: '',
        isSaving: false,
      };
    },
    computed: {
      hasPosts() {
        return this.posts.length > 0;
      },
      canPrevPage() {
        return this.page > 1;
      },
      canNextPage() {
        return this.page < this.pageCount;
      },
      pageLabel() {
        return `${this.page} / ${this.pageCount}`;
      },
    },
    mounted() {
      this.loadPosts(1);
    },
    methods: {
      loadPosts(page) {
        const url = `${window.apiRoot}/api/forum/posts?page=${page}`;

        return fetch(url)
          .then(response => {
            if (response.ok === false) {
              throw new Error(`HTTP ${response.status}`);
            }

            return response.json();
          })
          .then(result => {
            if (result.IsSuccess === false) {
              this.errorMessage = result.Message;
              return;
            }

            this.posts = result.Posts;
            this.page = result.Page;
            this.pageCount = result.PageCount;
            this.totalCount = result.TotalCount;
            this.errorMessage = '';
          })
          .catch(() => {
            this.errorMessage = '投稿一覧の取得に失敗しました。';
          });
      },
      createPost() {
        this.message = '';
        this.errorMessage = '';

        const params = new URLSearchParams();
        params.append('title', this.newTitle);
        params.append('body', this.newBody);

        this.isSaving = true;

        fetch(`${window.apiRoot}/api/forum/create`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8',
          },
          body: params.toString(),
        })
          .then(response => {
            if (response.ok === false) {
              throw new Error(`HTTP ${response.status}`);
            }

            return response.json();
          })
          .then(result => {
            if (result.IsSuccess === false) {
              this.errorMessage = result.Message;
              return;
            }

            this.newTitle = '';
            this.newBody = '';
            this.message = result.Message;

            return this.loadPosts(1);
          })
          .catch(() => {
            this.errorMessage = '投稿に失敗しました。';
          })
          .finally(() => {
            this.isSaving = false;
          });
      },
      movePage(page) {
        if (page < 1) {
          return;
        }

        if (page > this.pageCount) {
          return;
        }

        this.loadPosts(page);
      },
    },
  });
  {% end_javascript %}
</script>
{% end_area %}
```

### What this code does

- Keeps 20-item paging.
- Uses Vue + `fetch` exactly as required.
- Removes hard-coded front URLs.
- Keeps forum rendering simple and practical.

### Company rules followed

- MVC 5 + Liquid for front.
- Vue + `fetch` for client-side behavior.
- 20-item paging.
- Practical implementation without unnecessary architecture.

### Exact test steps after finishing this step

1. Log in.
2. Open `/forum`.
3. Confirm the first page of posts loads.
4. Create a new post.
5. Confirm the new post appears on page 1.
6. Create more than 20 posts total and confirm paging works.

### Expected result

The forum page works fully with login protection, post creation, list display, and 20-item paging.

### Common mistakes to avoid

- Do not change paging size from `20`.
- Do not use hard-coded `/Training.BBS/Web/w2.BBS.Front/...` URLs.
- Do not switch to jQuery AJAX.

## Step 5 - Build the Web Forms Manager Dashboard

### Goal

Implement the required manager-side admin screen using Web Forms, not MVC.

### Why this step comes now

The front side should be stable first. The manager side is currently the largest missing requirement.

### Files to edit manually

- `Web/w2.BBS.Manager/Global.asax.cs`
- `Web/w2.BBS.Manager/Default.aspx`
- `Web/w2.BBS.Manager/Default.aspx.cs`

### Exact file locations

- `Web/w2.BBS.Manager/Global.asax.cs`
- `Web/w2.BBS.Manager/Default.aspx`
- `Web/w2.BBS.Manager/Default.aspx.cs`

### Whether to replace the full file or only add code

- Replace full file for all three

### Exact full code block to type for `Web/w2.BBS.Manager/Global.asax.cs`

```csharp
// (c) 2025 W2 Co.,Ltd.

using System;
using w2.Common;

namespace w2.BBS.Manager
{
	public class Global : System.Web.HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
			Constants.APPLICATION_NAME = "w2.BBS.Manager";
			Constants.PHYSICALDIRPATH_LOGFILE = "C:\\Logs\\";
		}

		protected void Session_Start(object sender, EventArgs e)
		{
			this.Session["__DummyValueToFixSessionID__"] = string.Empty;
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{
		}

		protected void Application_Error(object sender, EventArgs e)
		{
		}

		protected void Session_End(object sender, EventArgs e)
		{
		}

		protected void Application_End(object sender, EventArgs e)
		{
		}
	}
}
```

### Exact full code block to type for `Web/w2.BBS.Manager/Default.aspx`

```aspx
<%@ Page Title="BBS Manager" Language="C#" MasterPageFile="~/Form/Common/Default.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="w2.BBS.Manager.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<h1>BBS Manager</h1>

	<% if (string.IsNullOrEmpty(this.ResultMessage) == false) { %>
	<p><%: this.ResultMessage %></p>
	<% } %>

	<h2>Users</h2>
	<p>Page <%: this.UserPageNo %> / <%: this.UserPageCount %> | Total <%: this.UserTotalCount %></p>

	<table border="1" cellpadding="4" cellspacing="0">
		<thead>
			<tr>
				<th>User ID</th>
				<th>Login ID</th>
				<th>User Name</th>
				<th>Mail Address</th>
				<th>Date Created</th>
				<th>Action</th>
			</tr>
		</thead>
		<tbody>
			<% foreach (var user in this.UserList) { %>
			<tr>
				<td><%: user.UserId %></td>
				<td><%: user.LoginId %></td>
				<td><%: user.UserName %></td>
				<td><%: user.MailAddress %></td>
				<td><%: user.DateCreatedText %></td>
				<td>
					<form method="post" action="Default.aspx?userPage=<%: this.UserPageNo %>&postPage=<%: this.PostPageNo %>" onsubmit="return confirm('Delete this user?');">
						<input type="hidden" name="actionType" value="delete-user" />
						<input type="hidden" name="targetId" value="<%: user.UserId %>" />
						<button type="submit">Delete</button>
					</form>
				</td>
			</tr>
			<% } %>
			<% if (this.UserList.Count == 0) { %>
			<tr>
				<td colspan="6">No users found.</td>
			</tr>
			<% } %>
		</tbody>
	</table>

	<p>
		<% if (this.UserPageNo > 1) { %>
		<a href="Default.aspx?userPage=<%: this.UserPageNo - 1 %>&postPage=<%: this.PostPageNo %>">Prev Users</a>
		<% } %>
		<% if (this.UserPageNo < this.UserPageCount) { %>
		<a href="Default.aspx?userPage=<%: this.UserPageNo + 1 %>&postPage=<%: this.PostPageNo %>">Next Users</a>
		<% } %>
	</p>

	<hr />

	<h2>Posts</h2>
	<p>Page <%: this.PostPageNo %> / <%: this.PostPageCount %> | Total <%: this.PostTotalCount %></p>

	<table border="1" cellpadding="4" cellspacing="0">
		<thead>
			<tr>
				<th>Post ID</th>
				<th>User Name</th>
				<th>Title</th>
				<th>Body</th>
				<th>Date Created</th>
				<th>Action</th>
			</tr>
		</thead>
		<tbody>
			<% foreach (var post in this.PostList) { %>
			<tr>
				<td><%: post.PostId %></td>
				<td><%: post.UserName %></td>
				<td><%: post.Title %></td>
				<td><%: post.Body %></td>
				<td><%: post.DateCreatedText %></td>
				<td>
					<form method="post" action="Default.aspx?userPage=<%: this.UserPageNo %>&postPage=<%: this.PostPageNo %>" onsubmit="return confirm('Delete this post?');">
						<input type="hidden" name="actionType" value="delete-post" />
						<input type="hidden" name="targetId" value="<%: post.PostId %>" />
						<button type="submit">Delete</button>
					</form>
				</td>
			</tr>
			<% } %>
			<% if (this.PostList.Count == 0) { %>
			<tr>
				<td colspan="6">No posts found.</td>
			</tr>
			<% } %>
		</tbody>
	</table>

	<p>
		<% if (this.PostPageNo > 1) { %>
		<a href="Default.aspx?userPage=<%: this.UserPageNo %>&postPage=<%: this.PostPageNo - 1 %>">Prev Posts</a>
		<% } %>
		<% if (this.PostPageNo < this.PostPageCount) { %>
		<a href="Default.aspx?userPage=<%: this.UserPageNo %>&postPage=<%: this.PostPageNo + 1 %>">Next Posts</a>
		<% } %>
	</p>
</asp:Content>
```

### Exact full code block to type for `Web/w2.BBS.Manager/Default.aspx.cs`

```csharp
// (c) 2025 W2 Co.,Ltd.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using w2.Common;

namespace w2.BBS.Manager
{
	public partial class Default : System.Web.UI.Page
	{
		private const int PAGE_SIZE = 20;

		private const string SQL_COUNT_USERS =
@"SELECT COUNT(1)
FROM w2_User
WHERE del_flg = 0";

		private const string SQL_SELECT_USERS =
@"SELECT
	user_id,
	login_id,
	user_name,
	mail_address,
	date_created
FROM w2_User
WHERE del_flg = 0
ORDER BY user_id DESC
OFFSET @offset ROWS
FETCH NEXT @page_size ROWS ONLY";

		private const string SQL_DELETE_USER =
@"UPDATE w2_User
SET
	del_flg = 1,
	date_changed = @date_changed
WHERE user_id = @user_id
AND del_flg = 0";

		private const string SQL_COUNT_POSTS =
@"SELECT COUNT(1)
FROM w2_ForumPost
WHERE del_flg = 0";

		private const string SQL_SELECT_POSTS =
@"SELECT
	p.post_id,
	u.user_name,
	p.title,
	p.body,
	p.date_created
FROM w2_ForumPost p
INNER JOIN w2_User u ON
(
	p.user_id = u.user_id
	AND u.del_flg = 0
)
WHERE p.del_flg = 0
ORDER BY
	p.date_created DESC,
	p.post_id DESC
OFFSET @offset ROWS
FETCH NEXT @page_size ROWS ONLY";

		private const string SQL_DELETE_POST =
@"UPDATE w2_ForumPost
SET
	del_flg = 1,
	date_changed = @date_changed
WHERE post_id = @post_id
AND del_flg = 0";

		protected List<UserRowModel> UserList { get; private set; } = new List<UserRowModel>();
		protected List<PostRowModel> PostList { get; private set; } = new List<PostRowModel>();
		protected int UserPageNo { get; private set; }
		protected int UserPageCount { get; private set; }
		protected int UserTotalCount { get; private set; }
		protected int PostPageNo { get; private set; }
		protected int PostPageCount { get; private set; }
		protected int PostTotalCount { get; private set; }
		protected string ResultMessage { get; private set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			this.HandlePostAction();

			this.UserPageNo = this.GetPageNo("userPage");
			this.PostPageNo = this.GetPageNo("postPage");

			this.LoadUsers();
			this.LoadPosts();
		}

		private void HandlePostAction()
		{
			if (this.IsPostBack == false)
			{
				return;
			}

			var actionType = (this.Request.Form["actionType"] ?? string.Empty).Trim();
			var targetIdText = (this.Request.Form["targetId"] ?? string.Empty).Trim();
			var userPageNo = this.GetPageNo("userPage");
			var postPageNo = this.GetPageNo("postPage");

			if (int.TryParse(targetIdText, out var targetId) == false)
			{
				return;
			}

			if (actionType == "delete-user")
			{
				this.DeleteUser(targetId);
				this.Response.Redirect($"Default.aspx?userPage={userPageNo}&postPage={postPageNo}", endResponse: true);
				return;
			}

			if (actionType == "delete-post")
			{
				this.DeletePost(targetId);
				this.Response.Redirect($"Default.aspx?userPage={userPageNo}&postPage={postPageNo}", endResponse: true);
			}
		}

		private int GetPageNo(string key)
		{
			var pageText = (this.Request.QueryString[key] ?? string.Empty).Trim();
			if (int.TryParse(pageText, out var pageNo) == false)
			{
				return 1;
			}

			return (pageNo < 1) ? 1 : pageNo;
		}

		private void LoadUsers()
		{
			this.UserTotalCount = this.GetCount(SQL_COUNT_USERS);
			this.UserPageCount = this.GetPageCount(this.UserTotalCount);

			if (this.UserPageNo > this.UserPageCount)
			{
				this.UserPageNo = this.UserPageCount;
			}

			var offset = (this.UserPageNo - 1) * PAGE_SIZE;
			var result = new List<UserRowModel>();

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_USERS, connection))
				{
					command.Parameters.AddWithValue("@offset", offset);
					command.Parameters.AddWithValue("@page_size", PAGE_SIZE);

					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							result.Add(new UserRowModel
							{
								UserId = Convert.ToInt32(reader["user_id"]),
								LoginId = Convert.ToString(reader["login_id"]),
								UserName = Convert.ToString(reader["user_name"]),
								MailAddress = Convert.ToString(reader["mail_address"]),
								DateCreatedText = Convert.ToDateTime(reader["date_created"]).ToString("yyyy/MM/dd HH:mm"),
							});
						}
					}
				}
			}

			this.UserList = result;
		}

		private void LoadPosts()
		{
			this.PostTotalCount = this.GetCount(SQL_COUNT_POSTS);
			this.PostPageCount = this.GetPageCount(this.PostTotalCount);

			if (this.PostPageNo > this.PostPageCount)
			{
				this.PostPageNo = this.PostPageCount;
			}

			var offset = (this.PostPageNo - 1) * PAGE_SIZE;
			var result = new List<PostRowModel>();

			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_SELECT_POSTS, connection))
				{
					command.Parameters.AddWithValue("@offset", offset);
					command.Parameters.AddWithValue("@page_size", PAGE_SIZE);

					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							result.Add(new PostRowModel
							{
								PostId = Convert.ToInt32(reader["post_id"]),
								UserName = Convert.ToString(reader["user_name"]),
								Title = Convert.ToString(reader["title"]),
								Body = Convert.ToString(reader["body"]),
								DateCreatedText = Convert.ToDateTime(reader["date_created"]).ToString("yyyy/MM/dd HH:mm"),
							});
						}
					}
				}
			}

			this.PostList = result;
		}

		private int GetCount(string sql)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(sql, connection))
				{
					return Convert.ToInt32(command.ExecuteScalar());
				}
			}
		}

		private void DeleteUser(int userId)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_DELETE_USER, connection))
				{
					command.Parameters.AddWithValue("@user_id", userId);
					command.Parameters.AddWithValue("@date_changed", DateTime.Now);
					command.ExecuteNonQuery();
				}
			}
		}

		private void DeletePost(int postId)
		{
			using (var connection = new SqlConnection(Constants.STRING_SQL_CONNECTION))
			{
				connection.Open();

				using (var command = new SqlCommand(SQL_DELETE_POST, connection))
				{
					command.Parameters.AddWithValue("@post_id", postId);
					command.Parameters.AddWithValue("@date_changed", DateTime.Now);
					command.ExecuteNonQuery();
				}
			}
		}

		private int GetPageCount(int totalCount)
		{
			if (totalCount == 0)
			{
				return 1;
			}

			return (int)Math.Ceiling((decimal)totalCount / PAGE_SIZE);
		}

		protected sealed class UserRowModel
		{
			public int UserId { get; set; }
			public string LoginId { get; set; }
			public string UserName { get; set; }
			public string MailAddress { get; set; }
			public string DateCreatedText { get; set; }
		}

		protected sealed class PostRowModel
		{
			public int PostId { get; set; }
			public string UserName { get; set; }
			public string Title { get; set; }
			public string Body { get; set; }
			public string DateCreatedText { get; set; }
		}
	}
}
```

### What this code does

- Keeps manager in Web Forms as required.
- Shows active users and active posts separately.
- Applies 20-item paging to both sections.
- Uses soft delete by updating `del_flg`.

### Company rules followed

- Admin side stays Web Forms.
- SQL is parameterized.
- Paging stays practical and consistent.
- No unnecessary architecture is introduced.

### Exact test steps after finishing this step

1. Open the manager site root.
2. Confirm user list shows registered users.
3. Confirm post list shows created posts.
4. Delete one user and confirm it disappears from the user list.
5. Delete one post and confirm it disappears from the post list.
6. Create more than 20 rows and confirm both user paging and post paging work.

### Expected result

The manager site is no longer placeholder content. It becomes a working admin screen with Web Forms and 20-item paging.

### Common mistakes to avoid

- Do not move the manager implementation into MVC.
- Do not hard-delete records when the existing schema uses `del_flg`.
- Do not forget to fix `APPLICATION_NAME`.
- Do not use more than 20 items per page.

## Step 6 - Clean Up Obsolete Sample Artifacts

### Goal

Remove final submission risk from old sample/demo code that is no longer part of the assignment flow.

### Why this step comes now

The main flow is already complete after Steps 1 to 5. This step removes leftover confusion and prevents broken sample paths from being reviewed as part of the assignment.

### Files to edit manually

- `Web/w2.BBS.Front/Xml/Db/Sample.xml`
- `Web/w2.BBS.Front/Views/index.liquid`

### Exact file locations

- `Web/w2.BBS.Front/Xml/Db/Sample.xml`
- `Web/w2.BBS.Front/Views/index.liquid`

### Whether to replace the full file or only add code

- Partial edit is enough

### Exact corrected code for `Web/w2.BBS.Front/Xml/Db/Sample.xml`

Replace only this line:

```xml
SET  message_test = @message_text
```

with:

```xml
SET  message_text = @message_text
```

### Exact corrected code for `Web/w2.BBS.Front/Views/index.liquid`

If you keep the file, replace the broken `res.status` references with `response.status`.

Replace:

```javascript
throw new Error(`HTTP ${res.status}`);
```

with:

```javascript
throw new Error(`HTTP ${response.status}`);
```

### What this code does

- Fixes the sample SQL typo.
- Removes obvious JavaScript mistakes from the leftover sample page.

### Company rules followed

- Keeps code simple.
- Avoids shipping clearly broken sample code.

### Exact test steps after finishing this step

1. Search the solution for `message_test`.
2. Search the solution for ``res.status``.
3. Confirm both are gone from active source files.

### Expected result

No clearly broken sample artifact remains in the submission.

### Common mistakes to avoid

- Do not spend time redesigning the sample page.
- Only remove the broken parts.

# 5. Mistakes Found In Current Project And How To Fix Them Manually

## 1. Broken catch-all route to a missing controller

- File name: `RouteConfig.cs`
- File location: `Web/w2.BBS.Front/App_Start/RouteConfig.cs`
- What is wrong:
  - It routes unmatched requests to `ErrorController.RedirectShortUrlOrDisplay404`, but that controller/action does not exist in the repository.
- Why it is wrong:
  - It causes runtime failure instead of graceful routing.
- Exact corrected code:
  - Use the full `RouteConfig.cs` from Step 2.
- Blocking or not:
  - Blocking.

## 2. Front root page is still demo/sample logic

- File name: `IndexController.cs`
- File location: `Web/w2.BBS.Front/Controller/IndexController.cs`
- What is wrong:
  - It still returns a sample message page and sample DB endpoints.
- Why it is wrong:
  - It does not match bulletin board assignment flow.
- Exact corrected code:
  - Use the full `IndexController.cs` from Step 2.
- Blocking or not:
  - High impact.

## 3. Front project file contains duplicate compile includes

- File name: `w2.BBS.Front.csproj`
- File location: `Web/w2.BBS.Front/w2.BBS.Front.csproj`
- What is wrong:
  - `ForumPostViewModel.cs`, `ForumListResponseViewModel.cs`, and `ForumActionResponseViewModel.cs` are included twice.
- Why it is wrong:
  - It is messy and may cause build confusion.
- Exact corrected code:

```xml
<Compile Include="ViewModels\ForumActionResponseViewModel.cs" />
<Compile Include="ViewModels\ForumIndexViewModel.cs" />
<Compile Include="ViewModels\ForumListResponseViewModel.cs" />
<Compile Include="ViewModels\ForumPostViewModel.cs" />
<Compile Include="ViewModels\IndexViewModel.cs" />
```

- Blocking or not:
  - Medium.

## 4. Auth views hard-code the IIS path

- File names:
  - `login.liquid`
  - `register.liquid`
- File locations:
  - `Web/w2.BBS.Front/Views/login.liquid`
  - `Web/w2.BBS.Front/Views/register.liquid`
- What is wrong:
  - Form actions and links are hard-coded to `/Training.BBS/Web/w2.BBS.Front/...`.
- Why it is wrong:
  - It breaks portability and ignores the shared `RootUrl`.
- Exact corrected code:

```liquid
action="{{ RootUrl | append: "/auth/login" }}"
```

```liquid
action="{{ RootUrl | append: "/auth/register" }}"
```

```liquid
href="{{ RootUrl | append: "/auth/login" }}"
```

```liquid
href="{{ RootUrl | append: "/auth/register" }}"
```

- Blocking or not:
  - High impact.

## 5. Password values are echoed back into HTML

- File names:
  - `login.liquid`
  - `register.liquid`
- File locations:
  - `Web/w2.BBS.Front/Views/login.liquid`
  - `Web/w2.BBS.Front/Views/register.liquid`
- What is wrong:
  - The password input uses `value="{{ Password }}"`.
- Why it is wrong:
  - It exposes sensitive input unnecessarily.
- Exact corrected code:

```liquid
<input type="password" id="password" name="Password" />
```

- Blocking or not:
  - Medium.

## 6. Debug DB-check action remains in auth controller

- File name: `AuthController.cs`
- File location: `Web/w2.BBS.Front/Controller/AuthController.cs`
- What is wrong:
  - `DbCheck()` is still present.
- Why it is wrong:
  - It is debug-only behavior and not part of the final assignment flow.
- Exact corrected code:
  - Remove the whole `DbCheck()` action and use the Step 3 full controller code.
- Blocking or not:
  - Medium.

## 7. Forum view hard-codes API URLs

- File name: `forum.liquid`
- File location: `Web/w2.BBS.Front/Views/forum.liquid`
- What is wrong:
  - It calls `/Training.BBS/Web/w2.BBS.Front/api/forum/...` directly.
- Why it is wrong:
  - It violates the root path strategy already prepared in `layout.liquid`.
- Exact corrected code:

```javascript
const url = `${window.apiRoot}/api/forum/posts?page=${page}`;
```

```javascript
fetch(`${window.apiRoot}/api/forum/create`, {
```

- Blocking or not:
  - High impact.

## 8. Manager application name is incorrect

- File name: `Global.asax.cs`
- File location: `Web/w2.BBS.Manager/Global.asax.cs`
- What is wrong:
  - It sets `Constants.APPLICATION_NAME = "w2.BBS.Front";`
- Why it is wrong:
  - Manager should identify itself correctly.
- Exact corrected code:

```csharp
Constants.APPLICATION_NAME = "w2.BBS.Manager";
```

- Blocking or not:
  - Medium.

## 9. Manager page is still placeholder content

- File names:
  - `Default.aspx`
  - `Default.aspx.cs`
- File locations:
  - `Web/w2.BBS.Manager/Default.aspx`
  - `Web/w2.BBS.Manager/Default.aspx.cs`
- What is wrong:
  - It only shows `Hello, World!`.
- Why it is wrong:
  - The admin side requirement is not implemented.
- Exact corrected code:
  - Use the full Web Forms code from Step 5.
- Blocking or not:
  - Blocking.

## 10. Sample XML uses the wrong column name

- File name: `Sample.xml`
- File location: `Web/w2.BBS.Front/Xml/Db/Sample.xml`
- What is wrong:
  - It writes to `message_test`.
- Why it is wrong:
  - The active sample query uses `message_text`.
- Exact corrected code:

```xml
SET  message_text = @message_text
```

- Blocking or not:
  - Low if sample flow is unused, medium if you keep the sample page alive.

## 11. Sample index page contains JavaScript errors

- File name: `index.liquid`
- File location: `Web/w2.BBS.Front/Views/index.liquid`
- What is wrong:
  - It throws using `res.status` even though the variable is named `response`.
- Why it is wrong:
  - It causes runtime JavaScript errors.
- Exact corrected code:

```javascript
throw new Error(`HTTP ${response.status}`);
```

- Blocking or not:
  - Low if the sample page is no longer used, but still should be fixed.

## 12. Required database template files are missing from the repository

- File name:
  - `bbs.mdf.template`
  - `bbs_log.ldf.template`
- File location:
  - `Web/w2.BBS.Front/App_Data/`
- What is wrong:
  - The README expects them, but they are absent.
- Why it is wrong:
  - This blocks environment setup and database availability.
- Exact corrected code:
  - Not a source-code fix. Restore the original files or manually create the database schema from Step 1.
- Blocking or not:
  - Blocking.

# 6. Final Completion Checklist

- [ ] Confirm IIS, Visual Studio admin mode, and ASP.NET State Service settings follow `README.md`
- [ ] Restore or create `Web/w2.BBS.Front/App_Data` database files
- [ ] Create `w2_User` table if missing
- [ ] Create `w2_ForumPost` table if missing
- [ ] Replace `Web/w2.BBS.Front/App_Start/RouteConfig.cs`
- [ ] Replace `Web/w2.BBS.Front/Controller/IndexController.cs`
- [ ] Remove duplicate compile includes from `Web/w2.BBS.Front/w2.BBS.Front.csproj`
- [ ] Replace `Web/w2.BBS.Front/Controller/AuthController.cs`
- [ ] Replace `Web/w2.BBS.Front/Views/login.liquid`
- [ ] Replace `Web/w2.BBS.Front/Views/register.liquid`
- [ ] Replace `Web/w2.BBS.Front/Controller/ForumController.cs`
- [ ] Replace `Web/w2.BBS.Front/Views/forum.liquid`
- [ ] Fix `Web/w2.BBS.Front/Xml/Db/Sample.xml`
- [ ] Fix `Web/w2.BBS.Front/Views/index.liquid` if you keep the file active
- [ ] Replace `Web/w2.BBS.Manager/Global.asax.cs`
- [ ] Replace `Web/w2.BBS.Manager/Default.aspx`
- [ ] Replace `Web/w2.BBS.Manager/Default.aspx.cs`
- [ ] Register a test user from the front site
- [ ] Log in from the front site
- [ ] Create forum posts from the front site
- [ ] Confirm front paging works at 20 items per page
- [ ] Open the manager site
- [ ] Confirm user list appears
- [ ] Confirm post list appears
- [ ] Confirm manager paging works at 20 items per page
- [ ] Confirm manager delete-user works
- [ ] Confirm manager delete-post works
- [ ] Confirm deleted rows are hidden because `del_flg = 1`
- [ ] Confirm front site still works after manager-side deletes
- [ ] Perform final manual IIS verification for both projects
