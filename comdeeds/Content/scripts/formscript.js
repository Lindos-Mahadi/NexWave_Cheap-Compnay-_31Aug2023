ko.subscribable.fn.trimmed = function () {
    return ko.computed({
        read: function () {
            //return this().trim();
            return this();
        },
        write: function (value) {
            this(value.trim());
            this.valueHasMutated();
        },
        owner: this
    }).extend({ notify: 'always' });
};

ko.validation.init({
    grouping: {
        deep: true,
        live: true,
        observable: true
    }
}, true);

ko.validation.rules['checked'] = {
    validator: function (value) {
        if (!value) {
            return false;
        }
        return true;
    }
};

ko.validation.registerExtenders();

window.onload = function () {
    initialize();
    var applicantdetails = function (fname, lname, email, ps, phone, Tuser) {
        this.firstname = ko.observable(fname).extend({
            required: true
        });;
        this.lastname = ko.observable(lname).extend({
            required: true
        });;
        this.email = ko.observable(email).extend({
            required: true,
            email: true
        });;
        this.pass = ko.observable(ps).extend({
            required: true
        });;
        this.phone = ko.observable(phone).extend({
            required: true,
            phone: true
        });;
        this.Tuser = ko.observable(Tuser).extend({
            required: true
        });;
    };

    var companydetails = function (companyName, reserve, companyABN, purpose, companyuse, state, declaration, trustname, ABN, TFN, trustaddress, trustcountry, tid, AcnasName, UlimateHoldingCompany, ucompanyname, acnarbnabn, countryIcor) {
        var s = this;
        s.companyName = ko.observable(companyName).extend({
            required: true
        });;
        s.tid = ko.observable(tid);
        s.AcnasName = ko.observable(AcnasName);
        s.reserve = ko.observable(reserve);
        s.companyabnpnl = ko.observable(false);
        s.companyABN = ko.observable(companyABN).extend({
            required: {
                onlyIf: function () { return s.reserve() === "yes" }
            }
        });;
        s.purpose = ko.observable(purpose).extend({
            required: true
        });;
        s.companyuse = ko.observable(companyuse).extend({
            required: true
        });;
        s.state = ko.observable(state).extend({
            required: true
        });;
        s.declaration = ko.observable(declaration).extend({
            required: {
                message: 'Declaration required !',
                onlyIf: function () { return s.companyuse() === "smsf" },
                param: true
            }
        });;
        s.trustname = ko.observable(trustname);
        s.ABN = ko.observable(ABN);
        s.TFN = ko.observable(TFN);
        s.trustaddress = ko.observable(trustaddress);
        s.trustcountry = ko.observable("");
        s.UlimateHoldingCompany = ko.observable(UlimateHoldingCompany);
        s.ucompanyname = ko.observable(ucompanyname);
        s.acnarbnabn = ko.observable(acnarbnabn);
        s.countryIcor = ko.observable(countryIcor);
     
    };

    var companyaddressdetails = function (id, unit, street, state, suburb, postcode, sameadd) {
        this.id = ko.observable(id);
        this.unit = ko.observable(unit);
        this.street = ko.observable(street).extend({
            required: true
        });;
        this.state = ko.observable(state).extend({
            required: true
        });;
        this.suburb = ko.observable(suburb).extend({
            required: true
        });;
        this.postcode = ko.observable(postcode).extend({
            required: true
        });;
        this.sameadd = ko.observable(sameadd);
    };

    var companyDiretors = function (id, fname, lname, dobday, dobmonth, dobyear, country, state, city, address, dirunit, dirstreet, dirsuburb, dirpostcode, dirstate) {
        var p = this;
        p.id = ko.observable(id);
        p.fname = ko.observable(fname).extend({
            required: true,
            minLength: 2
        });
        p.lname = ko.observable(lname).extend({
            required: true,
            minLength: 2
        });;
        p.dobday = ko.observable(dobday).extend({
            required: true
        });;
        p.dobmonth = ko.observable(dobmonth).extend({
            required: true
        });;
        p.dobyear = ko.observable(dobyear).extend({
            required: true
        });;
        p.country = ko.observable(country).extend({
            required: true
        });
        p.state = ko.observable(state).extend({
            required: {
                onlyIf: function () { return p.country() === "Australia" },
                param: true
            }
        });;
        p.stateraw = ko.observable(state).trimmed().extend({
            required: {
                onlyIf: function () { return p.country() !== "Australia" },
                param: true
            }
        });;

        p.city = ko.observable(city).extend({
            required: true,
            maxLength: 25
        });;
        p.address = ko.observable(address);

        p.dirunit = ko.observable(dirunit);

        p.dirstreet = ko.observable(dirstreet).extend({
            required: true
        });;
        p.dirsuburb = ko.observable(dirsuburb).extend({
            required: true
        });;
        p.dirpostcode = ko.observable(dirpostcode).extend({
            required: true
        });;
        p.dirstate = ko.observable(dirstate).extend({
            required: true
        });;
        return p;
    };

    var shareAllocation = function (id, dirname, did, shareOwner, shareType, noOfshare, shareAmount, shareoption, sharedetailsnotheldanotherorg, isheldanotherorg) {
        var a = this;
        a.Id = ko.observable(id);
        a.did = ko.observable(did);
        a.dirName = ko.observable(dirname);
        a.shareType = ko.observable(shareType).extend({ required: true });
        a.noOfshare = ko.observable(noOfshare).extend({ required: true, number: true });
        a.shareAmount = ko.observable(shareAmount).extend({ required: true, number: true });
        //a.rdocheck = ko.observable("true");
        var tf = "true";
        if (isheldanotherorg == "yes") {
            tf = "true";
        }
        else { tf = "false"; }
        a.rdocheck = ko.observable(tf);        
        a.shareoption = ko.observable(shareoption);        
        //a.rdocheck1 = ko.observable("true"); //update on 24-april-2017
        a.shareOwnerName = ko.observable(shareOwner); //.extend({ required: { onlyIf: function () { return a.rdocheck() === "true" } } });
        a.sharedetailsnotheldanotherorg = ko.observable(sharedetailsnotheldanotherorg); //.extend({ required: { onlyIf: function () { return a.rdocheck() === "false" } } });
        
        return a;
    };

    var IndivisualshareAllocation = function (id, rdocompORind, fullname, ind_dobday, ind_dobmonth, ind_dobyear, placeofbirth, indunit, indstreet, indsuburb, indstate, indpostcode, shareOwner, shareType, noOfshare, shareAmount, share_compORtrt, share_compORtrtname, abn, compunit, compstreet, compsuburb, compstate, comppostcode, compshareType, compnoOfshare, compshareAmount, compshareOwner) {
        var ind = this;
        ind.Id = ko.observable(id);
        // ind.rdocompORindd = ko.observable("yes");
        //ind.sharedirectorsCounter = ko.observable(sharedirectorsCounter);
        ind.rdshare_individual = ko.observable(rdocompORind);

        ind.share_indname = ko.observable(fullname).extend({
            required: true,
            minLength: 2
        });
        ind.ind_dobday = ko.observable(ind_dobday).extend({
            required: true
        });
        ind.ind_dobmonth = ko.observable(ind_dobmonth).extend({
            required: true
        });
        ind.ind_dobyear = ko.observable(ind_dobyear).extend({
            required: true
        });

        ind.share_placeofbirth = ko.observable(placeofbirth).extend({
            required: true,
            maxLength: 25
        });;

        ind.shareaddunit = ko.observable(indunit);

        ind.shareaddstreet = ko.observable(indstreet).extend({
            required: true
        });;
        ind.shareaddsuburb = ko.observable(indsuburb).extend({
            required: true
        });;

        ind.ddshareaddstate = ko.observable(indstate).extend({
            required: true
        });;
        ind.shareaddpostcode = ko.observable(indpostcode).extend({
            required: true
        });;
        ind.share_sharetype = ko.observable(shareType).extend({
            required: true
        });;
        ind.share_noofshare = ko.observable(noOfshare).extend({
            required: true, number: true
        });
        ind.share_amountopaidshare = ko.observable(shareAmount).extend({
            required: true, number: true
        });

        ind.share_indshareownername = ko.observable(shareOwner).extend({
            required: true
        });

        // ind.share_compORtrt = ko.observable(share_compORtrt);
        ind.share_compORtrtname = ko.observable(share_compORtrtname);
        ind.share_compORtrtABN = ko.observable(abn);

        ind.sharecompunit = ko.observable(compunit);
        ind.sharecompstreet = ko.observable(compstreet);
        ind.ddsharecompstate = ko.observable(compsuburb);
        ind.sharecompsuburb = ko.observable(compstate);
        ind.sharecomppostcode = ko.observable(comppostcode);

        ind.share_compsharetype = ko.observable(compshareType);
        ind.share_noofcompshare = ko.observable(compnoOfshare);
        ind.share_amountopaidcompshare = ko.observable(compshareAmount);
        ind.share_compshareownername = ko.observable(compshareOwner);

        return ind;
    }

    var companyshareAllocation = function (id, share_compORtrt, share_compORtrtname, abn, compunit, compstreet, compsuburb, compstate, comppostcode, compshareType, compnoOfshare, compshareAmount, compshareOwner,
        rdocompORind, fullname, ind_dobday, ind_dobmonth, ind_dobyear, placeofbirth, indunit, indstreet, indsuburb, indstate, indpostcode, shareOwner, shareType, noOfshare, shareAmount) {
        var comp = this;
        comp.Id = ko.observable(id);
        //comp.sharedirectorsCounter = ko.observable(sharedirectorsCounter);
        comp.rdshare_individual = ko.observable(share_compORtrt).extend({
            required: true
        });
        comp.share_compORtrtname = ko.observable(share_compORtrtname).extend({
            required: true, minLength: 4
        });
        comp.share_compORtrtABN = ko.observable(abn).extend({
            required: true,
            maxLength: 10
        });;
        comp.sharecompunit = ko.observable(compunit);

        comp.sharecompstreet = ko.observable(compstreet).extend({
            required: true
        });;
        comp.sharecompsuburb = ko.observable(compsuburb).extend({
            required: true
        });;

        comp.ddsharecompstate = ko.observable(compstate).extend({
            required: true
        });;
        comp.sharecomppostcode = ko.observable(comppostcode).extend({
            required: true
        });;
        comp.share_compsharetype = ko.observable(compshareType).extend({
            required: true
        });;
        comp.share_noofcompshare = ko.observable(compnoOfshare).extend({
            required: true, number: true
        });
        comp.share_amountopaidcompshare = ko.observable(compshareAmount).extend({
            required: true, number: true
        });

        comp.share_compshareownername = ko.observable(compshareOwner).extend({
            required: true
        });

        //  comp.rdocompORind = ko.observable(rdocompORind);
        comp.share_indname = ko.observable(fullname);
        comp.ind_dobday = ko.observable(ind_dobday);
        comp.ind_dobmonth = ko.observable(ind_dobmonth);
        comp.ind_dobyear = ko.observable(ind_dobyear);
        comp.share_placeofbirth = ko.observable(placeofbirth);
        comp.shareaddunit = ko.observable(indunit);
        comp.shareaddstreet = ko.observable(indstreet);
        comp.shareaddsuburb = ko.observable(indsuburb);
        comp.ddshareaddstate = ko.observable(indstate);
        comp.shareaddpostcode = ko.observable(indpostcode);
        comp.share_sharetype = ko.observable(shareType);
        comp.share_noofshare = ko.observable(noOfshare);
        comp.share_amountopaidshare = ko.observable(shareAmount);
        comp.share_indshareownername = ko.observable(shareOwner);

        return comp;
    }

    // update on April 23 2018
    var companyNonDiraddressdetails = function (id, unit, street, state, suburb, postcode, sameadd) {
        this.id = ko.observable(id);
        this.unit = ko.observable(unit);
        this.street = ko.observable(street).extend({
            required: true
        });;
        this.state = ko.observable(state).extend({
            required: true
        });;
        this.suburb = ko.observable(suburb).extend({
            required: true
        });;
        this.postcode = ko.observable(postcode).extend({
            required: true
        });;
        this.sameadd = ko.observable(sameadd);
    };

    function binndviews() {
        var self = this;

        self.ajxproc = ko.observable(false);
        self.applicant = ko.observableArray();
        self.company = ko.observableArray();
        self.companyAddress = ko.observableArray();
        self.companyPrincipalAddress = ko.observableArray();
        //self.companyNonDiraddressdetails = ko.observableArray();  
        self.pnlshowultimate = ko.observable(false);
        self.paddpnl = ko.observable(false);
        self.saddpnl = ko.observable(false);
        self.ndaddpnl = ko.observable(false);
        self.caddpnl1 = ko.observable(true);
        self.caddpnl2 = ko.observable(false);
        //self.paddpnlnd = ko.observable(false); 
        self.directors = ko.observableArray();
        self.shares = ko.observableArray();
        self.share_another = ko.observableArray();  
        self.directorsCounter = ko.observable("1");
        self.sharedirectorsCounter = ko.observable("0"); 
        self.directorformerrors = ko.observableArray();
        self.principleadderror = ko.observableArray();
        self.tabbing = ko.observable("0");
        self.txtAutocomplete1 = ko.observable("0");
        self.resultbinded = ko.observable(false);

        self.resclass = ko.observable("");
        self.stats = ko.observable("");
        self.desc = ko.observable("");
        self.estatus = ko.observable(false);
        self.rdonewshare = ko.observable("no");
        self.rdogovcheck = ko.observable("no");
        self.rdshare_individual = ko.observable("yes"); 
        //self.namuna = ko.observable(false);

        self.someBoolProperty = ko.observable(false);

        //self.ssss = function (e) {
        //    if (e.UlimateHoldingCompany)
        //    {
        //        self.namuna(false);
        //    }
        //    else
        //    {
        //        self.namuna(true);
        //    }
        //    return true;
        //}   
        self.someBoolProperty.subscribe(function (newValue) {
            if (newValue) {
                // do whatever you want to do for checked checkbox
                self.pnlshowultimate(true);
            } else {
                // unchecked
                self.pnlshowultimate(false);
            }
        }, self);
        //self.uhctoggle = function (e) {
        //    if (e == 'yes') {
        //        alert("Test1");
        //       //self.pnlshowultimate = true;
        //       self.pnlshowultimate(true);
        //    } else {
        //        alert("Test2");
        //       // self.pnlshowultimate = false;
        //       self.pnlshowultimate(false);
        //    }
        //}

        self.patoggle = function (e) {
            if (e == 'no') {
                self.paddpnl(true);
            } else {
                self.paddpnl(false);
            }
        };

        self.satoggle = function (e) {
            if (e == 'no') {
                self.saddpnl(false);
            } else {
                self.saddpnl(true);
            }
        };

        self.ctoggle = function (e) {  // update 22 may 2018
            if (e == 'no') {
                self.caddpnl1(false);
                self.caddpnl2(true);
            } else {
                self.caddpnl1(true);
                self.caddpnl2(false);
            }
        };

        // init form data
        self.applicant(new applicantdetails(user.fname, user.lname, user.email, "", user.phone, user.Tuser));
        //self.directors([new companyDiretors("", "", "", "", "", "", "", "", "")]);
        //self.shares([new shareAllocation("", "", "", "")]);
        // init data end

        // applicant detail
        self.applicantsubmit = function (f) {
            self.applicantformerror = ko.validation.group(self.applicant(), {
                deep: true
            });

            if (self.applicantformerror().length !== 0) {
                self.applicantformerror.showAllMessages();
            } else {
                return true;
            }
        }
        // applicant detail end

        //company details

        self.fetchCompany = function () {
            var cn = document.getElementById('hdncname').value;
            //alert(cn);
            $.ajax({
                type: "GET",
                beforeSend: function () { self.ajxproc(true); },
                complete: function () { self.ajxproc(false); },
                url: "/api/SiteApi/GetCompanyDetails/",
                contentType: "application/json",
                success: function (d) {
                    if (d.msg === "success") {
                        var c = d.company;
                        self.company([new companydetails(
                            c.companyName, c.isNameReserve ? "yes" : "no", c.abn,
                            c.companyPurpose, c.UseOfCompany, c.regState, c.decl,
                            c.trustName, c.trustAbn, c.trustTfn, c.trustAddress,
                            c.trustCountry, c.tid, c.AcnasName, c.UlimateHoldingCompany, c.ucompanyname, c.acnarbnabn, c.countryIcor)]);
                        if (c.UlimateHoldingCompany === "true" || c.UlimateHoldingCompany === "True") { self.someBoolProperty(true); self.pnlshowultimate(true); }
                        else { self.pnlshowultimate(false); }
                    } else if (d.msg === "null") {
                        self.company([new companydetails(cn, "no", "", "", "", "", "false", "", "", "", "", "", 0, true, false, "", "", "")]);
                        self.pnlshowultimate(false);
                    } else {
                        toastr.error("Error occurred on server, please try again.");
                    }
                    self.tabbing(2);
                },
                error: function () {
                    toastr.error("Server error");
                }
            });
        };

        self.companysubmit = function (f) {
            self.companyformerror = ko.validation.group(self.company(), {
                deep: true
            });

            //console.log(self.companyformerror().length);
            if (self.companyformerror().length !== 0) {
                self.companyformerror.showAllMessages();
            } else {
                var c = self.company()[0];
                var dt = JSON.stringify({
                    companyName: c.companyName(),
                    isNameReserve: c.reserve() === "yes",
                    abn: c.companyABN(),
                    companyPurpose: c.purpose(),
                    UseOfCompany: c.companyuse(),
                    regState: c.state(),
                    decl: c.declaration(),
                    trustName: c.trustname(),
                    trustAbn: c.ABN(),
                    trustTfn: c.TFN(),
                    trustAddress: c.trustaddress(),
                    trustCountry: c.trustcountry(),
                    AcnasName: c.AcnasName(),
                    UlimateHoldingCompany: c.UlimateHoldingCompany(),
                    ucompanyname: c.ucompanyname(),
                    acnarbnabn: c.acnarbnabn(),
                    countryIcor: c.countryIcor()
                });

                $.ajax({
                    type: "POST",
                    beforeSend: function () { self.ajxproc(true); },
                    complete: function () { self.ajxproc(false); },
                    url: "/api/SiteApi/AddCompany/",
                    data: dt,
                    contentType: "application/json",
                    success: function (d) {
                        if (d.msg == "success") {
                            toastr.success("Company added successfully");
                            self.toggletab("3");
                        } else {
                            toastr.error("Error occured on server, please try again.");
                        }
                    },
                    error: function () {
                        toastr.error("Server error");
                    }
                });
            }
        };
        //company detail end

        //company address detail
        self.fetchAddress = function () {
            $.ajax({
                type: "GET",
                beforeSend: function () { self.ajxproc(true); },
                complete: function () { self.ajxproc(false); },
                url: "/api/SiteApi/GetCompanyAddress/",
                contentType: "application/json",
                success: function (d) {
                    if (d.msg == "success") {
                        var a = d.regadd;
                        var p = d.principleadd;
                        if (a.sameadd == "no") {
                            self.paddpnl(true);
                        }
                        else {
                            self.paddpnl(false);
                        }

                        self.companyAddress(new companyaddressdetails(a.id, a.unit, a.street, a.state, a.suburb, a.postcode, a.sameadd));
                        self.companyPrincipalAddress(new companyaddressdetails(p.id, p.unit, p.street, p.state, p.suburb, p.postcode, ""));
                    } else if (d.msg == "null") {
                        self.companyAddress(new companyaddressdetails(0, "", "", "", "", "", "yes"));
                        self.companyPrincipalAddress(new companyaddressdetails(0, "", "", "", "", "", "yes"));
                    } else {
                        toastr.error("Error occured on server, please try again.");
                    }
                    self.tabbing(3);
                },
                error: function () {
                    toastr.error("Server error");
                }
            });
        };

        self.companyaddsubmit = function () {
            var isvalid = false;
            self.companyaddformerror = ko.validation.group(self.companyAddress(), {
                deep: true
            });
            self.principleadderror = ko.validation.group(self.companyPrincipalAddress(), {
                deep: true
            });
            if (self.companyaddformerror().length !== 0) {
                isvalid = false;
                self.companyaddformerror.showAllMessages();
            } else {
                isvalid = true;
            }

            if (document.getElementById('rdosamecompadd2').checked) {
                if (self.principleadderror().length !== 0) {
                    isvalid = false;
                    self.principleadderror.showAllMessages();
                } else {
                    isvalid = true;
                }
            }
            if (isvalid) {
                var a = self.companyAddress();
                var p = self.companyAddress().sameadd() == "yes" ? self.companyAddress() : self.companyPrincipalAddress();

                var dt = JSON.stringify([{
                    id: $.trim(a.id()),
                    unit: $.trim(a.unit()),
                    street: $.trim(a.street()),
                    state: $.trim(a.state()),
                    suburb: $.trim(a.suburb()),
                    postcode: $.trim(a.postcode()),
                    type: 'r',
                    sameadd: self.companyAddress().sameadd(),
                }, {
                    id: $.trim(p.id()),
                    unit: $.trim(p.unit()),
                    street: $.trim(p.street()),
                    state: $.trim(p.state()),
                    suburb: $.trim(p.suburb()),
                    postcode: $.trim(p.postcode()),
                    type: 'p',
                    sameadd: self.companyAddress().sameadd(),
                }]);

                $.ajax({
                    type: "POST",
                    beforeSend: function () { self.ajxproc(true); },
                    complete: function () { self.ajxproc(false); },
                    url: "/api/SiteApi/addcompanyaddress/",
                    data: dt,
                    contentType: "application/json",
                    success: function (d) {
                        if (d.msg == "success") {
                            toastr.success("Company address added successfully");
                            self.toggletab(4);
                        }
                        else if (d.msg == "Address not found") {
                            toastr.error("Address not found.");
                        }
                        else {
                            toastr.error("Error occured on server, please try again.");
                        }
                    },
                    error: function () {
                        toastr.error("Server error");
                    }
                });
            }
        };
        //company address detail

        // Company directors details

        self.fetchDirector = function () {
            $.ajax({
                type: "GET",
                beforeSend: function () { self.ajxproc(true); },
                complete: function () { self.ajxproc(false); },
                url: "/api/SiteApi/GetDirectors/",
                contentType: "application/json",
                success: function (d) {
                    if (d.msg == "success") {
                        self.directorsCounter(d.directors.length);
                        var t = [];
                        for (var i = 0; i < d.directors.length; i++) {
                            var s = d.directors[i];
                            t.push(new companyDiretors(s.id, s.fname, s.lname, s.dobday, s.dobmonth, s.dobyear, s.country, s.state, s.city, s.address, s.dirunit, s.dirstreet, s.dirsuburb, s.dirpostcode, s.dirstate));
                        }
                        self.directors(t);
                    } else if (d.msg == "null") {
                        $.ajax({
                            type: "GET",
                            beforeSend: function () { self.ajxproc(true); },
                            complete: function () { self.ajxproc(false); },
                            url: "/api/SiteApi/GetCompanyAddress/",
                            contentType: "application/json",
                            success: function (d) {
                                if (d.msg == "success") {
                                    var a = d.regadd;
                                    var p = d.principleadd;
                                    //self.companyAddress(new companyaddressdetails(a.id, a.unit, a.street, a.state, a.suburb, a.postcode, a.sameadd));
                                    //self.companyPrincipalAddress(new companyaddressdetails(p.id, p.unit, p.street, p.state, p.suburb, p.postcode, ""));
                                    self.directors([new companyDiretors(0, "", "", "", "", "Australia", "", "", "", "", a.unit, a.street, a.suburb, a.postcode, a.state)]);
                                }
                                else {
                                    self.directors([new companyDiretors(0, "", "", "", "", "", "", "", "", "", "", "", "", "", "")]);
                                }
                            },
                            error: function () {
                                toastr.error("Server error");
                            }
                        });

                        // self.directors([new companyDiretors(0, "", "", "", "", "", "", "", "", "","","","","","")]);
                    } else {
                        toastr.error("Error occured on server, please try again.");
                    }
                    self.tabbing(4);
                },
                error: function () {
                    toastr.error("Server error");
                }
            });
        };

        self.directorsCounter.subscribe(function (n) {
            var sl = self.directors().length;
            if (n != "" && parseInt(n) != sl) {
                if (parseInt(n) > sl) {
                    var unit = "";
                    var suburb = "";
                    var street = "";
                    var postcode = "";
                    var state = "";
                    $.ajax({
                        type: "GET",
                        beforeSend: function () { self.ajxproc(true); },
                        complete: function () { self.ajxproc(false); },
                        url: "/api/SiteApi/GetCompanyAddress/",
                        contentType: "application/json",
                        async: false,
                        success: function (d) {
                            if (d.msg == "success") {
                                var a = d.regadd;
                                var p = d.principleadd;
                                //self.companyAddress(new companyaddressdetails(a.id, a.unit, a.street, a.state, a.suburb, a.postcode, a.sameadd));
                                //self.companyPrincipalAddress(new companyaddressdetails(p.id, p.unit, p.street, p.state, p.suburb, p.postcode, ""));
                                //self.directors([new companyDiretors(0, "", "", "", "", "Australia", "", "", "", "", a.unit, a.street, a.suburb, a.postcode, a.state)]);
                                unit = a.unit;
                                suburb = a.suburb;
                                street = a.street;
                                postcode = a.postcode;
                                state = a.state;
                            }
                            else {
                                // self.directors([new companyDiretors(0, "", "", "", "", "", "", "", "", "", "", "", "", "", "")]);
                            }
                        },
                        error: function () {
                            toastr.error("Server error");
                        }
                    });
                    for (var i = sl; i < n; i++) {
                        self.directors.push(new companyDiretors(0, "", "", "", "", "Australia", "", "", "", "", unit, street, suburb, postcode, state));
                    }
                } else if (parseInt(n) < sl) {
                    for (var i = n; i < sl; i++) {
                        self.directors.pop();
                    }
                }
            } else if (!n) {
                self.directors([]);
            }
            //if (parseInt(n) > 0) {
            //    self.directors([]);
            //    for (var i = 0; i < parseInt(n) ; i++) {
            //        self.directors.push(new companyDiretors(0,"", "", "", "", "", "", "", "", ""));
            //    }
            //}
        });

        self.directorsubmit = function () {
            //console.log(self.directors());

            self.directorformerrors = ko.validation.group(self.directors(), {
                deep: true
            });
            if (self.directorformerrors().length !== 0) {
                self.directorformerrors.showAllMessages();
                //}
                //if (1== 0) {
                //    self.directorformerrors.showAllMessages();
            }
            else {
                //var r = [];
                //for (var a = 0; a < self.directors().length; a++)
                //{
                //    var n = self.directors()[a];
                //    r.push({
                //        id:n.id(),
                //        fname: n.fname(),
                //        lname: n.lname(),
                //        dobday: parseInt(n.dobday()),
                //        dobmonth: parseInt(n.dobmonth()),
                //        dobyear: parseInt(n.dobyear()),
                //        address: n.address(),
                //        city: n.city(),
                //        state: n.state(),
                //        country:n.country()
                //    });
                //}

                var dt = JSON.stringify(ko.toJS(self.directors()));

                $.ajax({
                    type: "POST",
                    beforeSend: function () { self.ajxproc(true); },
                    complete: function () { self.ajxproc(false); },
                    url: "/api/SiteApi/adddirectors/",
                    data: dt,
                    contentType: "application/json",
                    success: function (d) {
                        if (d.msg == "success") {
                            toastr.success("Company directors added successfully");
                            self.toggletab(5);
                        }
                        else if (d.msg == "Address not found") {
                            toastr.error("Address not found.");
                        }
                        else if (d.msg == "Duplicate") {
                            toastr.error("Company director can not be duplicate.");
                        }
                        else {
                            toastr.error("Error occured on server, please try again.");
                        }
                    },
                    error: function () {
                        toastr.error("Server error");
                    }
                });
            }
        };
        // Company directors details end

        // Share allocation

        self.fetchShares = function () {
            $.ajax({
                type: "GET",
                beforeSend: function () { self.ajxproc(true); },
                complete: function () { self.ajxproc(false); },
                url: "/api/SiteApi/GetShare/",
                contentType: "application/json",

                success: function (d) {
                    if (d.msg == "success") {
                        var t = [];
                        for (var i = 0; i < d.shares.length; i++) {
                            var s = d.shares[i];
                            t.push(new shareAllocation(s.Id, s.dirName, s.dId, s.ownername, s.shareclass, s.noofshare, s.sharecost, s.shareoption, s.sharedetailsnotheldanotherorg, s.isheldanotherorg));
                        }
                        self.shares([]);
                        self.shares(t);

                        $.ajax({
                            type: "GET",
                            beforeSend: function () { self.ajxproc(true); },
                            complete: function () { self.ajxproc(false); },
                            url: "/api/SiteApi/GetIndShare/",
                            contentType: "application/json",

                            success: function (d) {
                                if (d.msg == "success") {
                                    var t = [];
                                    var count = "";
                                    for (var i = 0; i < d.shares.length; i++) {
                                        var ss = d.shares[i]; count = ss.sharedirectorsCounter;
                                        if (ss.rdshare_individual == "true") {
                                            t.push(new IndivisualshareAllocation(ss.Id, "yes", ss.share_indname, ss.ind_dobday, ss.ind_dobmonth, ss.ind_dobyear, ss.share_placeofbirth, ss.shareaddunit, ss.shareaddstreet, ss.shareaddsuburb, ss.ddshareaddstate, ss.shareaddpostcode, ss.share_indshareownername, ss.share_sharetype, ss.share_noofshare, ss.share_amountopaidshare, "", "", "", "", "", "", "", "", "", "", "", ""));
                                        }
                                        else if (ss.rdshare_company == "true") {
                                            t.push(new companyshareAllocation(ss.Id, "no", ss.share_compORtrtname, ss.share_compORtrtABN, ss.sharecompunit, ss.sharecompstreet, ss.sharecompsuburb, ss.ddsharecompstate, ss.sharecomppostcode, ss.share_compsharetype, ss.share_noofcompshare, ss.share_amountopaidcompshare, ss.share_compshareownername, "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""));
                                        }
                                    }
                                    self.sharedirectorsCounter(count);
                                    self.share_another([]);
                                    self.share_another(t);
                                    if (count == 0)
                                    { self.rdonewshare("no"); self.saddpnl(false); }
                                    else { self.rdonewshare("yes"); self.saddpnl(true); }
                                }
                                else if (d.msg == "null") {
                                    self.share_another([new IndivisualshareAllocation(0, "yes", "", "", "", "", "", "", "", "", "", "", "", "ordinary", 1, 1, "", "", "", "", "", "", "", "", "ordinary", 1, 1, "")]);
                                }
                                else {
                                    toastr.error("Error occured on server, please try again.");
                                }
                                self.tabbing(5);
                            },
                            error: function () {
                                toastr.error("Server error");
                            }
                        });

                        $.ajax({
                            type: "GET",
                            beforeSend: function () { self.ajxproc(true); },
                            complete: function () { self.ajxproc(false); },
                            url: "/api/SiteApi/getConst/",
                            contentType: "application/json",
                            success: function (d1) {
                                var value = d1.split(",");

                                if (value[0] == "yes") {
                                    self.rdogovcheck("yes");
                                }
                                else if (value[0] == "" || value[0] == "no") {
                                    self.rdogovcheck("no");
                                }
                                else {
                                    toastr.error("Error occured on server, please try again.");
                                }

                                if (value[1] == "smsf") {
                                    self.ndaddpnl(false);// = false;
                                    self.saddpnl(false);
                                }
                                else {
                                    self.ndaddpnl(true);// = true;
                                }
                            },
                            error: function (error) {
                                toastr.error("Server error");
                            }
                        });
                    }
                    else if (d.msg == "null") {
                        self.shares([new shareAllocation(0, "", "", "", "ordinary", 1, 1,"Unpaid","","")]);
                    }
                    else {
                        toastr.error("Error occured on server, please try again.");
                    }
                    self.tabbing(5);
                },
                error: function () {
                    toastr.error("Server error");
                }
            });

            // another share holder update by 25 april 2018

            // another share holder end
        };

        self.sharesubmit = function () {
            self.shareformerrors = ko.validation.group(self.shares(), {
                deep: true
            });
            if (self.shareformerrors().length !== 0) {
                self.shareformerrors.showAllMessages();
            }
            else {
                var r = [];
                for (var a = 0; a < self.shares().length; a++) {
                    var n = self.shares()[a];
                    r.push({
                        Id: n.Id(),
                        dirName: n.dirName(),
                        dId: n.did(),
                        shareclass: n.shareType(),
                        noofshare: n.noOfshare(),
                        sharecost: parseFloat(n.shareAmount()),
                        other: n.rdocheck(),
                        ownername: n.shareOwnerName(),
                        isheldanotherorg: n.rdocheck(),

                        ///by praveen
                        sharedetailsnotheldanotherorg: n.sharedetailsnotheldanotherorg(),                       
                        shareoption: n.shareoption()
                    });
                }

                var ns = [];
                for (var a = 0; a < self.share_another().length; a++) {
                    var n = self.share_another()[a];
                    ns.push({
                        Id: n.Id,
                        share_indname: n.share_indname(),
                        share_indORcompny: n.share_indORcompny,
                        share_ABN: n.share_ABN,
                        ind_dobday: n.ind_dobday(),
                        ind_dobmonth: n.ind_dobmonth(),
                        ind_dobyear: n.ind_dobyear(),
                        share_placeofbirth: n.share_placeofbirth(),
                        shareaddunit: n.shareaddunit(),
                        shareaddstreet: n.shareaddstreet(),
                        shareaddsuburb: n.shareaddsuburb(),
                        ddshareaddstate: n.ddshareaddstate(),
                        shareaddpostcode: n.shareaddpostcode(),
                        share_noofshare: n.share_noofshare(),
                        share_indshareownername: n.share_indshareownername(),
                        share_sharetype: n.share_sharetype(),
                        share_amountopaidshare: n.share_amountopaidshare(),
                        share_compORtrtname: n.share_compORtrtname(),
                        share_compORtrtABN: n.share_compORtrtABN(),
                        sharecompunit: n.sharecompunit(),
                        sharecompstreet: n.sharecompstreet(),
                        ddsharecompstate: n.ddsharecompstate(),
                        sharecompsuburb: n.sharecompsuburb(),
                        sharecomppostcode: n.sharecomppostcode(),
                        share_compsharetype: n.share_compsharetype(),
                        share_noofcompshare: n.share_noofcompshare(),
                        share_amountopaidcompshare: n.share_amountopaidcompshare(),
                        share_compshareownername: n.share_compshareownername()                        
                    });
                }

                //  var ga = self.share_const_gov();
                var sa = self.share_another();
                var dtr = JSON.stringify(r);
                var dtns = JSON.stringify(ns);

                $.ajax({
                    type: "POST",
                    beforeSend: function () { self.ajxproc(true); },
                    complete: function () { self.ajxproc(false); },
                    url: "/api/SiteApi/addshare/",
                    data: dtr,
                    contentType: "application/json",
                    success: function (d) {
                        if (d.msg == "success") {
                            if (document.getElementById("rdoNewShare1").checked) {
                                $.ajax({
                                    type: "POST",
                                    url: "/api/SiteApi/addshare1/",
                                    data: dtns,
                                    async: false,
                                    contentType: "application/json",
                                    success: function (d) {
                                        if (d.msg == "success") {
                                        } else {
                                            toastr.error("Error occured on server, please try again.");
                                        }
                                    },
                                    error: function (error) {
                                        toastr.error("Server error");
                                    }
                                });
                            }
                            if (document.getElementById("rdoNewCon2").checked) {
                                var ncf = [{ constfee: "yes" }];
                                var dtncf = JSON.stringify(ncf);
                                $.ajax({
                                    type: "POST",
                                    url: "/api/SiteApi/updateConst/",
                                    // data: dttt,
                                    async: false,
                                    contentType: "application/json",
                                    success: function (d1) {
                                        if (d1.msg == "success") {
                                            location.href = "/company-setup/summary";
                                        }
                                        else {
                                            toastr.error("Error occured on server, please try again.");
                                        }
                                    },
                                    error: function (error) {
                                        toastr.error("Server error");
                                    }
                                });
                            }
                            else {
                                location.href = "/company-setup/summary";
                            }
                        } else {
                            toastr.error("Error occured on server, please try again.");
                        }
                    },
                    error: function () {
                        toastr.error("Server error");
                    }
                });
            }
        };

        // share allocation end

        // another share holder update by 25 april 2018

        self.sharedirectorsCounter.subscribe(function (n) {
            var sl = self.share_another().length;
            if (n != "" && parseInt(n) != sl) {
                if (parseInt(n) > sl) {
                    for (var i = sl; i < n; i++) {
                        self.share_another.push(new IndivisualshareAllocation(i, "yes", "", "", "", "", "", "", "", "", "", "", "", "ordinary", 1, 1, "", "", "", "", "", "", "", "", "ordinary", 1, 1, ""));
                    }
                } else if (parseInt(n) < sl) {
                    for (var i = n; i < sl; i++) {
                        self.share_another.pop();
                    }
                }
            } else if (!n) {
                self.share_another([]);
            }
        });

        // another share holder end

        self.toggletab = function (i) {
            //alert(i);
            google.maps.event.addDomListener(window, 'load', initialize);
            initialize();
            var a = parseInt(i);
            switch (a) {
                case 2:
                    self.fetchCompany();
                    break;
                case 3:
                    self.fetchAddress();
                    break;
                case 4:
                    self.fetchDirector();
                    break;
                case 5:
                    self.fetchShares();
                    break;
                default:
                    self.tabbing(a);
                    break;
            }
        };
        self.toggletab(activepnl);

        self.checkname = function (f) {
            //var self = this;
            // self.txtname = ko.observable().extend({ pattern: '^[a-zA-Z0-9.&].$' });
            var c = self.company()[0];
            if (Boolean(c.companyName())) {
                $.ajax({
                    type: "GET",
                    beforeSend: function () { },
                    complete: function () { },
                    url: "/api/SiteApi/CheckName/?name=" + encodeURIComponent(c.companyName()),
                    contentType: "application/json",
                    success: function (d) {
                        $.magnificPopup.open({
                            items: {
                                src: '#result-modal'
                            },
                            type: 'inline',
                            modal: true
                        });
                        self.stats(d.status);
                        self.desc(Boolean(d.msg) ? d.msg : "");

                        if (d.status == "Available") {
                            $("#lnksetup").show();
                            self.resclass("notification success");
                            self.desc("'" + c.companyName() + "' is available to register.");
                            self.estatus(true);
                        } else if (d.status == "empty" || d.status == "error") {
                            self.resclass("notification error");
                        } else if (d.status == "Subject To Names Determination") {
                            $("#lnksetup").show();
                            self.resclass("notification warning");
                            self.desc("'" + c.companyName() + "' is available to register.")
                            self.estatus(true);
                        }
                        else {
                            self.resclass("notification warning");
                            self.estatus(false);
                        }
                        self.resultbinded(true);
                    },
                    error: function () {
                        alert("Server error !");
                    }
                });
            }
        };

        self.closeModal = function () {
            $.magnificPopup.close();
        };
    };
    ko.applyBindings(binndviews(), document.getElementById("bindview"))
};