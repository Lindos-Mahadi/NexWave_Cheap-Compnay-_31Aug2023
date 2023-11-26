<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Success.aspx.cs" Inherits="comdeeds.Success" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    Form201 submitted successfully. Documents under progress from ASIC. <br /><br />
        <a href="../">click go to Home</a>
        <%--<a href="3E3810647.pdf">Download Certificate Now</a>--%>
    </div>
         <div class='main-wrapper' style="width: 100%; background-color: White; display:none;">
                <div class="section">
                    <div class="col-md-12">
                        <div>
                            <div >

                                <div class="m_fullheading">
                                    <asp:HiddenField ID="hdncompanyid" runat="server" />
                                    <asp:HiddenField ID="hdncompanyname" runat="server" />
                                    <asp:Label ID="lblcompanyhead" runat="server" Text="Company Details"></asp:Label>
                                    <asp:Label ID="lblmsg" runat="server" Text="" ForeColor="Red" Font-Size="Small" CssClass="pull-right"></asp:Label>
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
                                <td>ASIC Response(RA55(Success)/RA56(Manual Decision)) : </td><td>
                                    <asp:Label ID="lblresponseType" runat="server" Text=""></asp:Label></td>
                                </tr>
                                 <tr>
                                <td>ASIC Document Number  </td><td>
                                    <asp:Label ID="lbldocno" runat="server" Text=""></asp:Label></td>
                                </tr>
                                
                                    <tr>
                                        <td>
                                            Certificate</td>
                                        <td>
                                          
                                            <asp:Literal ID="litcertificate" runat="server"></asp:Literal>
                                            &nbsp;
                                            <asp:LinkButton ID="lnkrequestforcertificate" runat="server" 
                                                onclick="lnkrequestforcertificate_Click" >Request to Certificate</asp:LinkButton>
                                        </td>
                                    </tr>
                                </table>
                                <div class="clearfix"></div>
                                    <asp:Panel ID="pnlerror" runat="server">
                                        <asp:Literal ID="literror" runat="server"></asp:Literal>
                                        <br />
                                        <asp:LinkButton ID="lnkretry" runat="server" onclick="lnkretry_Click">Retry Again with Form-201</asp:LinkButton>
                                    </asp:Panel>
                                  </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
    </form>
</body>
</html>
