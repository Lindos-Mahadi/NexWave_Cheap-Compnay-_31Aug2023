<%@ Page Title="" Language="C#" MasterPageFile="~/AdminC/Admin.Master" AutoEventWireup="true" CodeBehind="CompanyAsicDetails.aspx.cs" Inherits="comdeeds.AdminC.CompanyAsicDetails" %>
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
                        <div>
                            <div>

                                <div class="m_fullheading">
                                    <asp:HiddenField ID="hdnuserid" runat="server" />
                                    <asp:HiddenField ID="hdncompanyid" runat="server" />
                                    <asp:HiddenField ID="hdncompanyname" runat="server" />
                                    <asp:HiddenField ID="hdnRegid" runat="server" />
                                    <asp:Label ID="lblcompanyhead" runat="server" Text="Company Details"></asp:Label>
                                    <asp:Label ID="lblmsg" runat="server" Text="" ForeColor="Red" Font-Size="Small" CssClass="pull-right"></asp:Label>
                                    <span class="pull-right" style="font-size: 12px; font-weight: bold;"></span>
                                        <a href="/AdminC/UserSearchList.aspx" class="pull-right btn btn-default" style=" margin: 10px;"><i class="fa fa-arrow-left"></i> Back to Company List</a>
                                </div>
                                </div>
                                <div style="    margin-top: 2%;    margin-bottom: 2%;">
                                <div id="no-more-tables">
                                <table class="table table-bordered">
                                <tr>
                                <td>Company Name</td><td>
                                    <asp:Label ID="lblname" runat="server" Text=""></asp:Label></td>
                                </tr>
                                <tr>
                                <td>ACN</td><td>
                                    <asp:Label ID="lblACN" runat="server" Text=""></asp:Label></td>
                                </tr>
                                <tr>
                                <td>Current Status</td><td>
                                    <asp:Label ID="lblstatus" runat="server" Text=""></asp:Label></td>
                                </tr>
                                <tr>
                                <td>Recent Received File Name</td><td>
                                    <asp:Label ID="lblfilename" runat="server" Text=""></asp:Label></td>
                                </tr>
                                <tr>
                                <td>ASIC Response  </td><td>
                                    <asp:Label ID="lblresponseType" runat="server" Text=""></asp:Label></td>
                                </tr>
                                 <tr>
                                <td>ASIC Document Number  </td><td>
                                    <asp:Label ID="lbldocno" runat="server" Text=""></asp:Label></td>
                                </tr>
                                </tr>
                                    <tr>
                                        <td>
                                            Certificate</td>
                                        <td>
                                          
                                            <asp:Literal ID="litcertificate" runat="server"></asp:Literal>
                                            &nbsp;
                                            <asp:LinkButton ID="lnkrequestforcertificate" runat="server" 
                                                onclick="lnkrequestforcertificate_Click" >Request to Certificate</asp:LinkButton>
                                              &nbsp;  &nbsp;
                                            <asp:LinkButton ID="downloadCertificate" runat="server" ForeColor="white" Visible="false" OnClick="downloadCertificate_Click" style="text-transform:uppercase;padding: 5px;background-color:#37b358;"> Download Document</asp:LinkButton>
                                        </td>
                                      
                                    </tr>
                                </table>
                                <div class="clearfix"></div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                             <asp:Label ID="lbledge" runat="server" Text="Please select server type" style=" font-size: 16px;
    font-weight: 500;" Visible="true"></asp:Label>
                                        </div>
                                        <div class="col-sm-12">

                                            <asp:DropDownList ID="ddledge" runat="server" CssClass="selectpicker" Visible="true" style="padding: 8px;font-size: 15px;" AutoPostBack="true" OnSelectedIndexChanged="ddledge_SelectedIndexChanged">
                                            <asp:ListItem Text="connect with edge1 server" Value="edge1.uat.asic.gov.au" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="connect with edge2 server" Value="edge2.uat.asic.gov.au"></asp:ListItem>
                                        </asp:DropDownList>
                                             <asp:Panel ID="pnlerror" runat="server">
                                        <asp:Literal ID="literror" runat="server"></asp:Literal>
                                        <br />
                                          
                                        <asp:LinkButton ID="lnkretry" runat="server" onclick="lnkretry_Click">Retry Again with Form-201</asp:LinkButton>
                                    </asp:Panel>
                                        </div>
                                    </div>
                                    <div class="clearfix"></div>
                                   
                                  </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers >
            <asp:PostBackTrigger ControlID="downloadCertificate" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
