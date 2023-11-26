<%@ Page Title="" Language="C#" MasterPageFile="~/AdminC/Admin.Master" AutoEventWireup="true" CodeBehind="TransactionRequests.aspx.cs" Inherits="comdeeds.AdminC.TransactionRequests" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class='main-wrapper' style="width: 100%; background-color: White;">
                <div class="section">
                    <div class="col-md-12">
                        <div class="panel panel-default">
                            <div class="panel-body">
                                <div class="admin_head" style="text-transform:uppercase;">
                                    All Request Details of
                                    <asp:Label ID="lblcompanyname" runat="server" Text=""></asp:Label>
                                    <span class="pull-right" style="font-size: 12px; font-weight: bold;"></span>
                                        <a href="/AdminC/UserSearchList.aspx" class="pull-right btn btn-default" style=" margin: 10px;"><i class="fa fa-arrow-left"></i> Back to Company List</a>
                                </div>
                                <asp:GridView ID="gvlist" runat="server" AutoGenerateColumns="false" CssClass=" table-bordered table-striped table-condensed cf"
                                    Width="100%" EmptyDataText="No Request sent.">
                                    <Columns>
                                        <asp:TemplateField HeaderText="SNo." ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                                            ItemStyle-CssClass="numeric" ControlStyle-BackColor="#4a8dc5">
                                            <ItemTemplate>
                                                <div data-title="Sno.">
                                                    <b>

                                                        <%# Container.DataItemIndex + 1 %></b>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                         <asp:TemplateField HeaderText="Details" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                                            ItemStyle-CssClass="numeric" ControlStyle-BackColor="#4a8dc5">
                                            <ItemTemplate>
                                                <div data-title="Details">
                                                    
                                                        <%# Eval("sms") %>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                         <asp:TemplateField HeaderText="Date" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="numeric">
                                                <ItemTemplate>
                                                <div  data-title="Date">
                                                    <%# Eval("entrydate", "{0:dd/MM/yyyy}")%>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                    </Columns>
                                    <HeaderStyle CssClass="cf BGC" />
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
