<%@ Page Language="C#" MasterPageFile="~/Form/Common/Default.master" AutoEventWireup="true" CodeBehind="PostList.aspx.cs" Inherits="w2.BBS.Manager.PostList" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="cBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<asp:Literal ID="litTitle" runat="server" />

	<div class="search">
		<asp:Literal ID="litSearchLbl" runat="server" />
		<asp:TextBox ID="tbSearchKeyword" runat="server" />
		<asp:LinkButton ID="lbSearch" runat="server" OnClick="lbSearch_OnClick">検索</asp:LinkButton>
		<asp:LinkButton ID="lbSearchClear" runat="server" CausesValidation="false" OnClick="lbSearchClear_OnClick">クリア</asp:LinkButton>
	</div>

	<asp:Literal ID="litPostsSection" runat="server" />
	<table class="grid">
		<thead>
			<tr>
				<th><asp:Literal ID="litP1" runat="server" /></th>
				<th><asp:Literal ID="litP2" runat="server" /></th>
				<th><asp:Literal ID="litP3" runat="server" /></th>
				<th><asp:Literal ID="litP4" runat="server" /></th>
				<th><asp:Literal ID="litP5" runat="server" /></th>
				<th><asp:Literal ID="litP6" runat="server" /></th>
			</tr>
		</thead>
		<tbody>
			<asp:Repeater ID="rptPosts" runat="server" OnItemCommand="rptPosts_OnItemCommand">
				<ItemTemplate>
					<tr>
						<td><%# Eval("post_id") %></td>
						<td><%# Eval("login_id") %></td>
						<td><%# Eval("user_name") %></td>
						<td><%# Eval("title") %></td>
						<td><%# Eval("body") %></td>
						<td>
							<asp:LinkButton ID="lbDelPost" runat="server" Text="削除" CommandName="Del"
								CommandArgument='<%# Eval("post_id") %>' OnClientClick="return confirm('削除しますか？');" />
						</td>
					</tr>
				</ItemTemplate>
			</asp:Repeater>
		</tbody>
	</table>

	<asp:Literal ID="litRepliesSection" runat="server" />
	<table class="grid">
		<thead>
			<tr>
				<th><asp:Literal ID="litR1" runat="server" /></th>
				<th><asp:Literal ID="litR2" runat="server" /></th>
				<th><asp:Literal ID="litR3" runat="server" /></th>
				<th><asp:Literal ID="litR4" runat="server" /></th>
				<th><asp:Literal ID="litR5" runat="server" /></th>
				<th><asp:Literal ID="litR6" runat="server" /></th>
			</tr>
		</thead>
		<tbody>
			<asp:Repeater ID="rptReplies" runat="server" OnItemCommand="rptReplies_OnItemCommand">
				<ItemTemplate>
					<tr>
						<td><%# Eval("reply_id") %></td>
						<td><%# Eval("post_id") %></td>
						<td><%# Eval("login_id") %></td>
						<td><%# Eval("user_name") %></td>
						<td><%# Eval("body") %></td>
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
