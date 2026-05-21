<%@ Page Language="C#" MasterPageFile="~/Form/Common/Default.master" AutoEventWireup="true" CodeBehind="UserList.aspx.cs" Inherits="w2.BBS.Manager.UserList" %>
<%-- // (c) 2026 W2 Co.,Ltd. --%>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="cBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<h1>ユーザー一覧</h1>

	<div class="search">
		<span class="label">検索（ログインID・ユーザー名・ユーザーID）</span>
		<asp:TextBox ID="tbSearchKeyword" runat="server" />
		<asp:LinkButton ID="lbSearch" runat="server" OnClick="lbSearch_OnClick">検索</asp:LinkButton>
		<asp:LinkButton ID="lbSearchClear" runat="server" CausesValidation="false" OnClick="lbSearchClear_OnClick">クリア</asp:LinkButton>
	</div>

	<table class="grid">
		<thead>
			<tr>
				<th>ユーザーID</th>
				<th>ログインID</th>
				<th>ユーザー名</th>
				<th>詳細</th>
				<th>編集</th>
			</tr>
		</thead>
		<tbody>
			<asp:Repeater ID="rUsers" runat="server">
				<ItemTemplate>
					<tr>
						<td><%#: Eval("user_id") %></td>
						<td><%#: Eval("login_id") %></td>
						<td><%#: Eval("user_name") %></td>
						<td>
							<asp:LinkButton ID="lbUserDetail" runat="server" Text="詳細" CommandArgument='<%# Eval("user_id") %>' OnCommand="lbUserDetail_OnCommand" />
						</td>
						<td>
							<asp:LinkButton ID="lbUserEdit" runat="server" Text="編集" CommandArgument='<%# Eval("user_id") %>' OnCommand="lbUserEdit_OnCommand" />
						</td>
					</tr>
				</ItemTemplate>
			</asp:Repeater>
		</tbody>
	</table>
</asp:Content>
