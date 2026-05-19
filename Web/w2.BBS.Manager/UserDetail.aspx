<%@ Page Language="C#" MasterPageFile="~/Form/Common/Default.master" AutoEventWireup="true" CodeBehind="UserDetail.aspx.cs" Inherits="w2.BBS.Manager.UserDetail" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="cBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<asp:Literal ID="litTitle" runat="server" />
	<asp:Literal ID="litProfile" runat="server" />

	<asp:Literal ID="litPostsTitle" runat="server" />
	<table class="grid">
		<thead>
			<tr>
				<th><asp:Literal ID="litPc1" runat="server" /></th>
				<th><asp:Literal ID="litPc2" runat="server" /></th>
				<th><asp:Literal ID="litPc3" runat="server" /></th>
			</tr>
		</thead>
		<tbody>
			<asp:Repeater ID="rptPosts" runat="server" OnItemCommand="rptPosts_OnItemCommand">
				<ItemTemplate>
					<tr>
						<td><%# Eval("post_id") %></td>
						<td><%# Eval("title") %></td>
						<td>
							<asp:LinkButton ID="lbDelPost" runat="server" Text="削除" CommandName="DelPost"
								CommandArgument='<%# Eval("post_id") %>' OnClientClick="return confirm('この投稿を削除しますか？');" />
						</td>
					</tr>
				</ItemTemplate>
			</asp:Repeater>
		</tbody>
	</table>

	<asp:Literal ID="litRepliesTitle" runat="server" />
	<table class="grid">
		<thead>
			<tr>
				<th><asp:Literal ID="litRc1" runat="server" /></th>
				<th><asp:Literal ID="litRc2" runat="server" /></th>
				<th><asp:Literal ID="litRc3" runat="server" /></th>
				<th><asp:Literal ID="litRc4" runat="server" /></th>
			</tr>
		</thead>
		<tbody>
			<asp:Repeater ID="rptReplies" runat="server" OnItemCommand="rptReplies_OnItemCommand">
				<ItemTemplate>
					<tr>
						<td><%# Eval("reply_id") %></td>
						<td><%# Eval("post_id") %></td>
						<td><%# Eval("body") %></td>
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
