<%@ Page Language="C#" MasterPageFile="~/Form/Common/Default.master" AutoEventWireup="true" CodeBehind="UserList.aspx.cs" Inherits="w2.BBS.Manager.UserList" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="cBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<asp:Literal ID="litTitle" runat="server" />

	<div class="search">
		<asp:Literal ID="litSearchLbl" runat="server" />
		<asp:TextBox ID="tbSearchKeyword" runat="server" />
		<asp:LinkButton ID="lbSearch" runat="server" OnClick="lbSearch_OnClick">&#26908;&#32034;</asp:LinkButton>
		<asp:LinkButton ID="lbSearchClear" runat="server" CausesValidation="false" OnClick="lbSearchClear_OnClick">&#12463;&#12522;&#12450;</asp:LinkButton>
	</div>

	<table class="grid">
		<thead>
			<tr>
				<th><asp:Literal ID="litTh1" runat="server" /></th>
				<th><asp:Literal ID="litTh2" runat="server" /></th>
				<th><asp:Literal ID="litTh3" runat="server" /></th>
				<th><asp:Literal ID="litTh4" runat="server" /></th>
				<th><asp:Literal ID="litTh5" runat="server" /></th>
			</tr>
		</thead>
		<tbody>
			<asp:Repeater ID="rptUsers" runat="server">
				<ItemTemplate>
					<tr>
						<td><%# Eval("user_id") %></td>
						<td><%# Eval("login_id") %></td>
						<td><%# Eval("user_name") %></td>
						<td>
							<asp:LinkButton ID="lbUserDetail" runat="server" Text="&#35443;&#32048;" CommandArgument='<%# Eval("user_id") %>' OnCommand="lbUserDetail_OnCommand" />
						</td>
						<td>
							<asp:LinkButton ID="lbUserEdit" runat="server" Text="&#32232;&#38598;" CommandArgument='<%# Eval("user_id") %>' OnCommand="lbUserEdit_OnCommand" />
						</td>
					</tr>
				</ItemTemplate>
			</asp:Repeater>
		</tbody>
	</table>
</asp:Content>