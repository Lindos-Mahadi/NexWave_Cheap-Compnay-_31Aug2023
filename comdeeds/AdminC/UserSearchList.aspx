<%@ Page Title="" Language="C#" MasterPageFile="~/AdminC/Admin.Master" AutoEventWireup="true" CodeBehind="UserSearchList.aspx.cs" Inherits="comdeeds.AdminC.UserSearchList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function openModal() {
            $('#myModal').modal('show');
        }
    </script>
    <script type="text/javascript">

        function DownloadPDF() {

        }
        function executeAfter(val) {

            window.open("../ExportedFiles\\" + val + "\\Form201.pdf", "_blank");
            window.open("../ExportedFiles\\" + val + "\\Doc_" + val + ".zip", "_blank");

            return false;
        }
    </script>

    <style type="text/css">
        .parentDisable {
            position: fixed;
            left: 0;
            color: white;
            font-size: 25px;
            text-align: center;
            top: 0;
            background: #000;
            opacity: 0.8;
            z-index: 998;
            height: 100%;
            width: 100%;
            margin-top: 60px;
            padding: 20%;
        }

        #result .success, .notification {
            line-height: 24px;
            margin-bottom: 15px;
            position: relative;
            padding: 20px 26px;
            padding-right: 50px;
            border-radius: 3px;
        }

            .notification p {
                margin: 0;
            }

            #result .success, .notification.success {
                background: linear-gradient(to bottom,rgba(255,255,255,0.2),transparent);
                background-color: #EBF6E0;
            }

                #result .success, .notification.success, .notification.success a, .notification.success strong {
                    color: #5f9025;
                }

            .notification.error {
                background: linear-gradient(to bottom,rgba(255,255,255,0.2),transparent);
                background-color: #ffe9e9;
            }

                .notification.error, .notification.error a, .notification.error strong {
                    color: #de5959;
                }

            .notification.warning {
                background: linear-gradient(to bottom,rgba(255,255,255,0.2),transparent);
                background-color: #FBFADD;
            }

                .notification.warning, .notification.warning a, .notification.warning strong {
                    color: #8f872e;
                }

            .notification.notice h4 {
                font-size: 19px;
                margin: 3px 0 15px 0;
            }

            .notification.notice h4, .notification.notice, .notification.notice a, .notification.notice strong {
                color: #3184ae;
            }

            .notification.notice {
                background: linear-gradient(to bottom,rgba(255,255,255,0.2),transparent);
                background-color: #E9F7FE;
            }

                .notification.notice.large {
                    padding: 32px 36px;
                }

        body .notification strong {
            border: none;
        }

        .notification.success .close, .notification.error .close, .notification.warning .close, .notification.notice .close {
            padding: 0px 9px;
            position: absolute;
            right: 0;
            top: 22px;
            display: block;
            height: 8px;
            width: 8px;
            cursor: pointer;
        }

        .notification.success .close {
            background: url(../images/alert_boxes_close_ico.html) 0 -8px no-repeat;
        }

        .notification.error .close {
            background: url(../images/alert_boxes_close_ico.html) 0 0 no-repeat;
        }

        .notification.warning .close {
            background: url(../images/alert_boxes_close_ico.html) 0 -16px no-repeat;
        }

        .notification.notice .close {
            background: url(../images/alert_boxes_close_ico.html) 0 -24px no-repeat;
        }

        .notification.notice p span i {
            font-weight: 500;
        }

        .notification a.button {
            float: right;
            color: #fff;
            margin-top: 3px;
        }

        .notification.notice a.button {
            background-color: #388fc5;
        }

        .notification.warning a.button {
            background-color: #dfbe51;
        }

        .notification.error a.button {
            background-color: #d34c4c;
        }

        .notification.success a.button {
            background-color: #79ba38;
        }

        .notification.closeable a.close:before {
            content: "\f00d";
            font-family: "FontAwesome";
            position: absolute;
            right: 25px;
            top: 0;
            cursor: pointer;
        }

        td.numeric div > a {
            display: inline-block;
            width: 30px !important;
            background: #2b7cbb;
            border-radius: 5px;
            height: 32px;
        }

        }
    </style>
    <style>
        .GridPager a,
        .GridPager span {
            display: inline-block;
            padding: 0px 9px;
            margin-right: 4px;
            border-radius: 3px;
            border: solid 1px #c0c0c0;
            background: #e9e9e9;
            box-shadow: inset 0px 1px 0px rgba(255,255,255, .8), 0px 1px 3px rgba(0,0,0, .1);
            font-size: .875em;
            font-weight: bold;
            text-decoration: none;
            color: #717171;
            text-shadow: 0px 1px 0px rgba(255,255,255, 1);
        }

        .GridPager a {
            background-color: #f5f5f5;
            color: #969696;
            border: 1px solid #969696;
        }

        .GridPager span {
            background: #2b7cbb;
            box-shadow: inset 0px 0px 8px rgba(0,0,0, .5), 0px 1px 0px rgba(255,255,255, .8);
            color: #f0f0f0;
            text-shadow: 0px 0px 3px rgba(0,0,0, .5);
            border: 1px solid #3AC0F2;
        }
    </style>
    <style>
        * {
            -webkit-box-sizing: border-box;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
        }

            *:before, *:after {
                -webkit-box-sizing: border-box;
                -moz-box-sizing: border-box;
                box-sizing: border-box;
            }

        .modalPopup {
            background-color: #696969;
            filter: alpha(opacity=40);
            opacity: 0.7;
            xindex: -1;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <script language="javascript" type="text/javascript">

                ShowProcessMessage = function (PanelName) {
                    //Sets the visibility of the Div to 'visible'
                    document.getElementById(PanelName).style.visibility = "visible";

                    /* Displays the  'In-Process' message through the innerHTML.
                    You can write Image Tag to display Animated Gif */

                    document.getElementById(PanelName).innerHTML =
                        '<div class="parentDisable"><img src="../IMAGES/circle.gif" /></div>';

                    //Call Function to Disable all the other Controls
                    DisableAllControls('btnLoad');

                    return true; //Returns the control to the Server click event
                }
            </script>

            <script language="javascript" type="text/javascript">

                DisableAllControls = function (CtrlName) {
                    var elm;
                    /*Loop for all the controls of the page.*/

                    /* 1.Check for the Controls with type 'hidden' –
                    which are ASP.NET hidden controls for Viewstate and EventHandlers.
                    It is very important that these are always enabled,
                    for ASP.NET page to be working.
                    2.Also Check for the control which raised the event
                    (Button) - It should be active. */

                    elm = document.forms[0].elements[i];

                    if ((elm.name == CtrlName) || (elm.type == 'hidden')) {
                        elm.disabled = false;
                    }
                    else {
                        elm.disabled = true; //Disables all the other controls
                    }

                }
                <%--function DelCheck() {
                    if (confirm("Are you sure ? This will delete all data Permanently for this entry.")) {
                        document.getElementById('<%=btnDel123.ClientID %>').click();
                        location.reload(true);
                    }

                }--%>
            </script>
            <script type="text/javascript">
         // var prm = Sys.WebForms.PageRequestManager.getInstance();
          //Raised before processing of an asynchronous postback starts and the postback request is sent to the server.
        //  prm.add_beginRequest(BeginRequestHandler);
          // Raised after an asynchronous postback is finished and control has been returned to the browser.
         // prm.add_endRequest(EndRequestHandler);
       <%--   function BeginRequestHandler(sender, args) {
              //Shows the modal popup - the update progress
              var popup = $find('<%= modalPopup.ClientID %>');
              if (popup != null) {
                  popup.show();
              }
          } function EndRequestHandler(sender, args) {
              //Hide the modal popup - the update progress
              var popup = $find('<%= modalPopup.ClientID %>');
              if (popup != null) {
                  popup.hide();
              }
          }--%>

                function BeginRequestHandler() {
                    //Shows the modal popup - the update progress
                    var popup = $find('<%= modalPopup.ClientID %>');
                    if (popup != null) {
                        popup.show();
                    }
                }

          //     $('form').live("lnkasic", function () {
          //         alert("test");
          //    BeginRequestHandler();
          //});
            </script>

            <asp:HiddenField ID="hdnprogess" runat="server" />
            <asp:ModalPopupExtender ID="modalPopup" runat="server" TargetControlID="hdnprogess"
                PopupControlID="pnldd" BackgroundCssClass="modalPopup" />

            <asp:Panel ID="pnldd" runat="server">

                <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                    <ProgressTemplate>
                        <div style="position: fixed; float: right; z-index: 999; border: 1px solid #ccc; padding: 10px; border-radius: 10px 10px 10px 10px; display: block; margin-left: -10%; background: white; margin-top: 0%; font-weight: bold; font-size: 20px;">
                            <img src="../Content/images/asic-conn.gif" width="60px" />
                            Connecting to asic ...
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </asp:Panel>
            <div class='main-wrapper' style="width: 100%; background-color: White;">
                <div class="section">

                    <div id="ProcessingWindow" visible="false">
                    </div>

                    <div class="col-md-12">
                        <div class="panel panel-default" style="padding-top: 7px;">
                            <div class="panel-body">
                                <div class="admin_head">
                                    Current Company List
                                    <asp:LinkButton class="btn btn-default" ID="btnDel123" runat="server" Style="visibility: hidden;" OnClick="btnDel_Click">
                                    </asp:LinkButton>

                                    <asp:LinkButton class="btn btn-default" ID="btnDel" runat="server" Visible="false" OnClick="btnDel_Click" OnClientClick="if (!confirm('Are you sure ? This will delete all data Permanently for this entry.')) return false;" >
                        <i class="mdi mdi-delete"></i>  Delete
                                    </asp:LinkButton>
                                    <span class="pull-right" style="font-size: 12px; font-weight: bold; margin-top: -5px;">
                                        <%--<asp:LinkButton ID="lnkpaid" runat="server" ForeColor="Green"
                                        onclick="lnkpaid_Click">Paid</asp:LinkButton> |
                                    <asp:LinkButton ID="lnkunpaid" runat="server" ForeColor="Red"
                                        onclick="lnkunpaid_Click"  >Unpaid</asp:LinkButton>--%>

                                        <asp:CheckBox ID="chkpaid" runat="server" Text="Paid" />&nbsp;&nbsp;
                                    <asp:CheckBox ID="chkunpaid" runat="server" Text="Unpaid" />&nbsp;&nbsp;

                                    <asp:CheckBox ID="chkasicerror" runat="server" Text="Asic Errors" />
                                        &nbsp;&nbsp;
                                    <asp:CheckBox ID="chkasicsuccess" runat="server" Text="Asic Success" />

                                        <asp:LinkButton ID="lnksearch" runat="server" ForeColor="Green" CssClass="btn btn-default btn-sm"
                                            OnClick="lnksearch_Click" Style="margin-top: -6px; margin-left: 12px; width: 90px; border: 1px solid #98b9ee; font-weight: bold;">Search</asp:LinkButton>
                                    </span>
                                    <span class="pull-right col-md-4">
                                        <asp:TextBox ID="txtemail" runat="server" CssClass="form-control pull-left input-sm" Style="width: 96%; float: left!important; margin-top: -10px; font-weight: normal; height: 32px;" MaxLength="50"></asp:TextBox>
                                        <asp:LinkButton ID="LinkButton1" runat="server" placeholder='Customer Email-Id'
                                            Style="font-size: 20px; margin-left: -25px; float: left; margin-top: -6px;"
                                            OnClick="lnksearchu_Click"><span class="glyphicon glyphicon-search "></span></asp:LinkButton>
                                    </span>
                                </div>

                                <asp:GridView ID="gvcompanylist" runat="server" AutoGenerateColumns="false" CssClass=" table-bordered table-striped table-condensed cf " Width="100%" OnRowCommand="gvcompanylist_RowCommand" AllowPaging="true"
                                    OnPageIndexChanging="gvcompanylist_PageIndexChanging" PageSize="10" ShowHeaderWhenEmpty="true" EmptyDataText="No records found." EmptyDataRowStyle-CssClass="text-center" OnRowDataBound="gvcompanylist_RowDataBound">
                                    <PagerStyle HorizontalAlign="Right" CssClass="GridPager" />
                                    <Columns>

                                        <asp:TemplateField>

                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chkBxHeader" runat="server" AutoPostBack="true" OnCheckedChanged="chkBxHeader_CheckedChanged" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="cbSelect" runat="server" OnCheckedChanged="chkSelect_CheckedChanged" AutoPostBack="true" Visible='<%#Eval("status").ToString()=="paid"?false:true %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="SNo." ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="numeric" ControlStyle-BackColor="#4a8dc5">
                                            <ItemTemplate>
                                                <%-- <div  data-title="Sno.">--%>
                                                <div>
                                                    <%--<b> <%# Container.DataItemIndex + 1 %></b> --%>

                                                    <b>
                                                        <a href="TransactionRequests.aspx?cid=<%# Eval("id") %>&cname=<%# Eval("fullname") %>" title="ASIC STATUS DETAILES"><%# Container.DataItemIndex + 1 %></a>
                                                    </b>

                                                    <asp:HiddenField ID="hdnid" runat="server" Visible="false" Value='<%# Eval("id") %>' />
                                                    <asp:HiddenField ID="hdnfullname" runat="server" Value='<%# Eval("fullname") %>' />
                                                    <asp:HiddenField ID="hdnStatus" runat="server" Value='<%# Eval("status") %>' />
                                                    <asp:HiddenField ID="hdnRegid" runat="server" Visible="false" Value='<%# Eval("Regid") %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Company Name" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="numeric">
                                            <ItemTemplate>
                                                <div data-title="Company">
                                                    <asp:LinkButton ID="lnkdms" Visible='<%#Eval("Asic_ResType").ToString().ToUpper()=="RA55".ToUpper()?true:false %>' runat="server" CausesValidation="true" OnClick="lnkdms_Click" Style="display: none;">
    <span class="glyphicon glyphicon-folder-open"></span>
                                                    </asp:LinkButton>
                                                    &nbsp;&nbsp;
                                                    <asp:Label ID="lblcompanyname" runat="server" Text='<%# Eval("companyname") %>' Style="font-weight: bold;"></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="User" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="numeric">
                                            <ItemTemplate>
                                                <div data-title="Company">
                                                    <asp:Label ID="lbluserid" runat="server" Text='<%# Eval("userid") %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="numeric">
                                            <ItemTemplate>
                                                <div data-title="Status">
                                                    <%# Eval("status") %>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Date" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="numeric">
                                            <ItemTemplate>
                                                <div data-title="Date">
                                                    <%# Eval("searchon", "{0:dd/MM/yyyy}")%>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="center" HeaderStyle-HorizontalAlign="center" ControlStyle-Width="100px" ItemStyle-Width="15%" ItemStyle-CssClass="numeric">
                                            <ItemTemplate>

                                                <%--<asp:LinkButton ID="lnkview" runat="server" OnClick="lnkview_Click"
                                                    Visible='<%#Eval("status").ToString().ToUpper()=="Unpaid".ToUpper()?true:false %>'>Complete Now</asp:LinkButton>

                                                   <asp:LinkButton ID="lnkdownload" Visible='<%#Eval("Asic_ResType").ToString().ToUpper()=="RA55".ToUpper()?true:false %>'       runat="server"  CausesValidation="true" OnClick="lnkdownload_Click" OnClientClick="DownloadPDF()">Documents</asp:LinkButton>
                                                   <asp:Button ID="btnemaildocument" Visible='<%#Eval("Asic_ResType").ToString().ToUpper()=="RA55".ToUpper()?true:false %>' runat="server" Text="Send Email" style="background-color:#5f9fcf; border-color:#5f9fcf; color:White; width:80px; " CssClass="btn btn-success btn-sm" CausesValidation="true"
                                                OnClick="btnemaildocument_Click" OnClientClick ="ShowProcessMessage('ProcessingWindow')" />

                                                    <asp:Label ID="lblemailsent" runat="server" Text= "" Visible="false" style="line-height: 2;color:red;"></asp:Label>--%>
                                                <%--<cc1:ModalPopupExtender ID="mp1" runat="server" PopupControlID="Panel1" TargetControlID="lnkview"
                                                CancelControlID="btnClose" BackgroundCssClass="modalBackground">
                                            </cc1:ModalPopupExtender>
                                            <asp:Panel ID="Panel1" runat="server" CssClass="modalPopup" align="center" Style="display: none">
                                                This is an ASP.Net AJAX ModalPopupExtender Example<br />
                                                <asp:Button ID="btnClose" runat="server" Text="Close" />
                                            </asp:Panel>--%>
                                                <asp:UpdatePanel ID="uup" runat="server">
                                                    <ContentTemplate>

                                                        <div data-role="display">
                                                            <asp:LinkButton ID="LinkButton2" CommandName="DownloadCertificate" CommandArgument='<%#Eval("id")%>' runat="server" title="Download Certificate" CausesValidation="true"><i class="icon mdi mdi-file" style="    color: white;font-size: 20px;padding: 8px;"></i></asp:LinkButton>
                                                            <asp:LinkButton ID="LinkButton3" CommandName="downloadconstitution" CommandArgument='<%#Eval("id")%>' runat="server" title="Download Constitution" Visible='<%#((Eval("govofcomapany").ToString()=="yes") || (Eval("userid").ToString()=="superinsure1@gmail.com"))?true:false %>' CausesValidation="true"><i class="icon mdi mdi-timer" style=" color: white; font-size: 20px; padding: 8px;"></i></asp:LinkButton>
                                                            <asp:LinkButton ID="LinkButton4" CommandName="downloadreginvoice" CommandArgument='<%#Eval("id")%>' runat="server" ToolTip="Download Reginvoice" CausesValidation="true"><i class="icon mdi mdi-apps" style="    color: white; font-size: 20px; padding: 8px;"></i></asp:LinkButton>
                                                            <asp:LinkButton ID="LinkButton5" CommandName="DownloadASICCertificate" CommandArgument='<%#Eval("id")%>' runat="server" ToolTip="Download ASIC Certificate" CausesValidation="true"><i class="icon mdi mdi-download" style="    color: white; font-size: 20px; padding: 8px;"></i></asp:LinkButton>
                                                        </div>
                                                    </ContentTemplate>
                                                    <Triggers>
                                                        <asp:PostBackTrigger ControlID="LinkButton2" />
                                                        <asp:PostBackTrigger ControlID="LinkButton3" />
                                                        <asp:PostBackTrigger ControlID="LinkButton4" />
                                                        <asp:PostBackTrigger ControlID="LinkButton5" />
                                                    </Triggers>
                                                </asp:UpdatePanel>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="numeric">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkasic" runat="server" OnClick="lnkasic_Click" ForeColor="Blue" ToolTip=" Asic Status" OnClientClick="BeginRequestHandler()" Visible='<%#Eval("status").ToString()=="Unpaid"?false:true %>'>  <span class="label label-warning"> Asic Status</span></asp:LinkButton>
                                                <asp:LinkButton ID="lnkasic1" runat="server" OnClick="lnkasic1_Click" ForeColor="Blue" ToolTip="Pay here" Visible='<%#Eval("status").ToString()=="Unpaid"?true:false %>'>  <span class="label label-info"> Pay Now</span></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="numeric">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkshowdetials" runat="server" ForeColor="Green" ToolTip=" Show Company Details" OnClick="lnkshowdetials_Click"
                                                    Visible='<%#Eval("status").ToString().ToUpper()=="paid".ToUpper()?true:false %>'> <span class="label label-success"> Show Details</span></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="numeric" Visible="false">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkdelete" runat="server" OnClick="lnkdelete_Click" ForeColor="Red" OnClientClick="return confirm('Are you sure you want remove');"
                                                    Visible='<%#Eval("status").ToString()=="Unpaid"?true:false %>'><span class="label label-warning">Remove</span></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="numeric" Visible="false">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnksend" runat="server" OnClick="lnksend_Click" ForeColor="Green" Visible='<%#Eval("status").ToString()=="Unpaid"?false:true %>'>Send Mail</asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="numeric">
                                            <ItemTemplate>
                                                <%--<asp:LinkButton ID="lnkedit" runat="server" OnClick="lnkedit_Click" ForeColor="Green" ToolTip=" Edit Company Details" Visible='<%#Eval("status").ToString()=="Unpaid"?false:true %>'> <span class="label label-default">Edit</span></asp:LinkButton>--%>
                                                <asp:LinkButton ID="lnkedit" runat="server" OnClick="lnkedit_Click" ForeColor="Green" ToolTip=" Edit Company Details"> <span class="label label-default">Edit</span></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <HeaderStyle CssClass="cf BGC" />
                                </asp:GridView>
                                <div class="panel-footer">
                                    <div id="errormsg" runat="server"></div>
                                    <%-- <asp:Label ID="errormsg" runat="server" ForeColor="Red"></asp:Label>--%>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- Trigger the modal with a button -->
    <button type="button" class="btn btn-info btn-lg" data-toggle="modal" data-target="#myModal"
        style="display: none;">
        Open Modal</button>
    <!-- Modal -->
    <div id="myModal" class="modal fade" role="dialog">
        <div class="modal-dialog" style="width: 98%; margin: 10px 5px;">
            <!-- Modal content-->
            <div class="modal-content">

                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <iframe id="dmsid" runat="server" width="100%" height="515px" src="" style="border: 0px solid white;"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">
                        Close</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>