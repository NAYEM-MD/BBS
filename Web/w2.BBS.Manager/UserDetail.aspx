<%@ Page Language="C#" MasterPageFile="~/Form/Common/Default.master" AutoEventWireup="true" CodeBehind="UserDetail.aspx.cs" Inherits="w2.BBS.Manager.UserDetail" %>
<%-- // (c) 2026 W2 Co.,Ltd. --%>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="cBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<h1>ユーザー詳細</h1>

	<asp:Panel ID="pnlNotFound" runat="server" CssClass="error" Visible="false">
		<p>ユーザーが見つかりません。</p>
	</asp:Panel>

	<asp:Panel ID="pnlProfile" runat="server" Visible="false">
		<p><span class="label">ユーザーID</span><asp:Literal ID="lUserId" runat="server" /></p>
		<p><span class="label">ログインID</span><asp:Literal ID="lLoginId" runat="server" /></p>
		<p><span class="label">ユーザー名</span><asp:Literal ID="lUserName" runat="server" /></p>
	</asp:Panel>

	<h2>このユーザーの投稿</h2>
	<table class="grid">
		<thead>
			<tr>
				<th>投稿ID</th>
				<th>タイトル</th>
				<th></th>
			</tr>
		</thead>
		<tbody>
			<asp:Repeater ID="rPosts" runat="server" OnItemCommand="rPosts_OnItemCommand">
				<ItemTemplate>
					<tr>
						<td><%#: Eval("post_id") %></td>
						<td><%#: Eval("title") %></td>
						<td>
							<asp:LinkButton ID="lbDelPost" runat="server" Text="削除" CommandName="DelPost"
								CommandArgument='<%# Eval("post_id") %>' OnClientClick="return confirm('この投稿を削除しますか？');" />
						</td>
					</tr>
				</ItemTemplate>
			</asp:Repeater>
		</tbody>
	</table>

	<h2>このユーザーの返信</h2>
	<table class="grid">
		<thead>
			<tr>
				<th>返信ID</th>
				<th>投稿ID</th>
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
						<td><%#: Eval("body") %></td>
						<td>
							<asp:LinkButton ID="lbDelReply" runat="server" Text="削除" CommandName="DelReply"
								CommandArgument='<%# Eval("reply_id") %>' OnClientClick="return confirm('この返信を削除しますか？');" />
						</td>
					</tr>
				</ItemTemplate>
			</asp:Repeater>
		</tbody>
	</table>

	<asp:LinkButton ID="lbBack" runat="server" CausesValidation="false" OnClick="lbBack_OnClick">ユーザー一覧へ</asp:LinkButton>
</asp:Content>
