<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RA54_Asic.aspx.cs" Inherits="comdeeds.RA54_Asic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <div class="row" style="    margin-left: 72px;width:90%;float:left;">
    <div class="col-md-12">
      <h1 style="margin: -12px 0px 12px 0px;font-size: 22px;text-transform:uppercase;">RA54 Lodgement</h1>
    </div>
<div class="col-md-12">
     <div class="panel panel-primary" style="display:block;">
  <div class="panel-heading">Declaration</div>
  <div class="panel-body">
  
      <table  style="width:95%;">
          <tr>
              <td>
                  <asp:CheckBox ID="CheckBox1" runat="server" Text=" I declare that the holder of the attached X.509 certificate is a person authorised to digitally sign transmissions to ASIC on behalf of this lodging agent" Style="font-weight: normal !important;color: #337ab7;" />
              </td>
          </tr>
         <tr>
              <td style="font-weight:normal;">
                  <asp:CheckBox ID="CheckBox2" runat="server" Text=" I declare that the holder of the attached X.509 certificate is a person authorised to digitally sign company forms which this lodging agent may transmit to ASIC" Style="font-weight: normal;color: #337ab7;"/>
              </td>
          </tr>
          <tr>
              <td><br /><br />
                   <asp:Button ID="btnra54" runat="server" Text="Continue" 
          CssClass="btn btn-primary" onclick="btnra54_Click"  />
                
              </td>
          </tr>
      </table>
  <asp:Label ID="lblmsg" runat="server" style="FONT-family: verdana;"></asp:Label>


  </div> 
  </div>
    </div></div>
</asp:Content>
