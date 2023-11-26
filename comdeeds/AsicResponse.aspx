<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AsicResponse.aspx.cs" Inherits="comdeeds.AsicResponse" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
     .m_fullheading {
    padding: 2px 0px;
    float: left;
    width: 100%;
    font-weight: 800;
    border-bottom: 4px solid rgba(74, 141, 197, 0.72);
    margin-bottom: 15px;
}
</style>
     <link rel="stylesheet" type="text/css" href="../Areas/Admin/assets/lib/perfect-scrollbar/css/perfect-scrollbar.min.css" />
    <link rel="stylesheet" type="text/css" href="../Areas/Admin/assets/lib/material-design-icons/css/material-design-iconic-font.min.css" />
    <!--[if lt IE 9]>
    <script src="https://oss.maxcdn.com/html5shiv/3.7.2/html5shiv.min.js"></script>
    <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
    <![endif]-->


    <link rel="stylesheet" href="../Areas/Admin/assets/css/style.css" type="text/css" />
    <link rel="stylesheet" href="../Areas/Admin/assets/css/grid.min.css" type="text/css" />
    <link rel="stylesheet" href="../Areas/Admin/assets/css/admin.css" type="text/css" />

    <script src="../Areas/Admin/assets/lib/jquery/jquery.min.js" type="text/javascript"></script>
    <script src="../Areas/Admin/assets/js/grid.min.js" type="text/javascript"></script>
    <script src="../Areas/Admin/assets/js/knockout-3.4.2.js" type="text/javascript"></script>
 <%--   <script src="~/Areas/Admin/assets/js/knockout-validation.js" type="text/javascript"></script>--%>

    <script src="../Areas/Admin/assets/js/knockout-validation.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">

    <div class='main-wrapper' style="width: 100%; background-color: White;">
        <div class="section">
            <div class="col-md-12">
                <a href="admin/dashboard"><img src="/Content/images/logo.jpg" alt="" style="height: 50px;" /></a>
                <div class="m_fullheading">
                    <asp:HiddenField ID="hdncompanyid" runat="server" />
                    <asp:HiddenField ID="hdncompanyname" runat="server" />
                    <asp:HiddenField ID="hdnEmail" runat="server" />
                    <asp:Label ID="lblcompanyhead" runat="server" Text="ASIC RESPONSE"></asp:Label><br />
                </div>
            </div>
            <div class="col-md-12 btn btn-alert">
                <asp:Label ID="lblmsg" runat="server" Text="" ForeColor="white" CssClass="pull-left" Style=" color: #09679e;font-size: 16px;"></asp:Label>
            </div>
            <div class="col-md-12">
                <div class="col-md-6 pull-left">

                     <asp:LinkButton ID="lnkretry" runat="server" OnClick="lnkretry_Click" CssClass="btn btn-success"
                    Style="margin-top: 20px;text-transform:uppercase;">Click here to re-submit</asp:LinkButton>
                    &nbsp;&nbsp;&nbsp;
                    <asp:LinkButton ID="lnkexit" runat="server" PostBackUrl="AdminC/UserSearchList.aspx" CssClass="btn btn-primary"
                    Style="color: white; margin-top: 20px;text-transform:uppercase;"> Click here to Back to Admin </asp:LinkButton>
                </div>
                  <div class="col-md-6 ">
                       
                  </div>
    
            </div>
            <div class="col-md-12">
                <asp:Label ID="lblstatus" runat="server" Text="" ForeColor="Blue" CssClass="pull-left" Visible="false"></asp:Label>
                <br />
                <br />
            </div>
            <div class="col-md-12">
                <asp:Literal ID="literror" runat="server"></asp:Literal>
                <br />
                <br />
                <br />
                <br />
                <br />
            </div>
        </div>
       
    </div>
    </form>
</body>
</html>
