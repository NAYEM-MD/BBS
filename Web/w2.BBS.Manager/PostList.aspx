<%@ Page Language="C#" MasterPageFile="~/Form/Common/Default.master" AutoEventWireup="true" CodeBehind="PostList.aspx.cs" Inherits="w2.BBS.Manager.PostList" %>
<%-- // (c) 2026 W2 Co.,Ltd. --%>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="cBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<h1>投稿・返信一覧</h1>

	<div class="search">
		<span class="label">検索（ログインID・ユーザー名・本文・タイトル・ID）</span>
		<asp:TextBox ID="tbSearchKeyword" runat="server" />
		<asp:LinkButton ID="lbSearch" runat="server" OnClick="lbSearch_OnClick">検索</asp:LinkButton>
		<asp:LinkButton ID="lbSearchClear" runat="server" CausesValidation="false" OnClick="lbSearchClear_OnClick">クリア</asp:LinkButton>
	</div>

	<h2>投稿</h2>
	<table class="grid">
		<thead>
			<tr>
				<th>投稿ID</th>
				<th>ログインID</th>
				<th>ユーザー名</th>
				<th>タイトル</th>
				<th>本文</th>
				<th></th>
			</tr>
		</thead>
		<tbody>
			<asp:Repeater ID="rPosts" runat="server" OnItemCommand="rPosts_OnItemCommand">
				<ItemTemplate>
					<tr>
						<td><%#: Eval("post_id") %></td>
						<td><%#: Eval("login_id") %></td>
						<td><%#: Eval("user_name") %></td>
						<td><%#: Eval("title") %></td>
						<td><%#: Eval("body") %></td>
						<td>
							<asp:LinkButton ID="lbDelPost" runat="server" Text="削除" CommandName="Del"
								CommandArgument='<%# Eval("post_id") %>' OnClientClick="return confirm('削除しますか？');" />
						</td>
					</tr>
				</ItemTemplate>
			</asp:Repeater>
		</tbody>
	</table>

	<h2>返信</h2>
	<table class="grid">
		<thead>
			<tr>
				<th>返信ID</th>
				<th>投稿ID</th>
				<th>ログインID</th>
				<th>ユーザー名</th>
				<th>本文</th>
				<th></th>
			</tr>
		</thead>
		<tbody>
			<asp:Repeater ID="rReplies" runat="server" OnItemCommand="rReplies_OnItemCommand">
				<ItemTemplate>
					<tr>
						<td><%#: Eval("reply_id") %></td>
						<td><%#: Eval("post_id") %></td>
						<td><%#: Eval("login_id") %></td>
						<td><%#: Eval("user_name") %></td>
						<td><%#: Eval("body") %></td>
						<td>
							<asp:LinkButton ID="lbDelReply" runat="server" Text="削除" CommandName="Del"
								CommandArgument='<%# Eval("reply_id") %>' OnClientClick="return confirm('削除しますか？');" />
						</td>
					</tr>
				</ItemTemplate>
			</asp:Repeater>
		</tbody>
	</table>
</asp:Content>
