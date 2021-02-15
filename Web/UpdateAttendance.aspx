<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UpdateAttendance.aspx.cs" MasterPageFile="~/global.Master" Inherits="gradproj_webapp.UpdateAttendance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
    </style>

    <script src="Scripts.js" type="text/javascript"></script>

    <script type="text/javascript">
        var Profile_Object;
        $(document).ready(function () {
        });
    </script>
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <lable id="lbl_user" class="h5"></lable>
    <div id="class_control" class="input-group pr-3">
        <div class="input-group-prepend">
            <label id="lbl_class" class="input-group-text" for="select_month">Class</label>
        </div>
        <select class="custom-select" id="select_class">
            <option value="0">Select one.....</option>
        </select>
    </div>    
    
    <div id="otpcontainer" class="otp container-fluid center-block">
        <lable id="lbl_otp" class="otptextarea"></lable>
        <br/>
        <button type="button"  class="btn btn-primary" id="btn_generateotp" onclick="generateOTP();">Generate OTP</button>
    </div>
</asp:Content>
