﻿(function (n) {
    "use strict";

    function t(t) {
        var r, i;
        this.config = {};
        n.extend(this, u);
        t && n.extend(this, t);
        r = {};
        for (i in this.output_fields) this.output_fields[i] !== undefined && (r[i] = n(this.output_fields[i]));
        this.$output_fields = r
    }

    function f(n) {
        var t = /\D/g;
        return n.sort(function (n, i) {
            return parseInt(n.replace(t, ""), 10) - parseInt(i.replace(t, ""), 10)
        }), n
    }
    var i = [],
        r, u = {
            api_key: "",
            output_fields: {
                line_1: "#line1",
                line_2: "#line2",
                line_3: "#line3",
                post_town: "#town",
                county: "#county",
                postcode: "#postcode"
            },
            api_endpoint: "https://api.getAddress.io/v2/uk",
            input: undefined,
            $input: undefined,
            input_label: "Enter your Postcode",
            input_muted_style: "color:#CBCBCB;width:60%;display:inline;",
            input_class: "",
            input_id: "opc_input",
            button: undefined,
            $button: undefined,
            button_id: "opc_button",
            button_label: "Find your Address",
            button_class: "",
            button_disabled_message: "Fetching...",
            $dropdown: undefined,
            dropdown_id: "opc_dropdown",
            dropdown_select_message: "Select your Address",
            dropdown_class: "form-control",
            $error_message: undefined,
            error_message_id: "opc_error_message",
            error_message_postcode_invalid: "Please recheck your postcode, it seems to be incorrect",
            error_message_postcode_not_found: "Your postcode could not be found. Please type in your address",
            error_message_default: "We were not able to your address from your Postcode. Please input your address manually",
            error_message_class: "",
            lookup_interval: 1e3,
            debug_mode: !1,
            onLookupSuccess: undefined,
            onLookupError: undefined,
            onAddressSelected: undefined,
            sort_numerically: !1
        };
    Array.prototype.clean = function (n) {
        for (var t = 0; t < this.length; t++) this[t] == n && (this.splice(t, 1), t--);
        return this
    };
    String.prototype.trim || (String.prototype.trim = function () {
        return this.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, "")
    });
    t.prototype.setupPostcodeInput = function (n) {
        this.$context = n;
        this.setupInputField();
        this.setupLookupButton()
    };
    t.prototype.setupInputField = function () {
        var t = this;
        return this.$input = n(this.input).length ? n(this.input).first() : n("<input />", {
            type: "text",
            id: this.input_id,
            class: 'form-control',
            value: this.input_label
        }).appendTo(this.$context).addClass(this.input_class).val(this.input_label).attr("style", this.input_muted_style).attr("autocomplete", "off").submit(function () {
            return !1
        }).keypress(function (n) {
            n.which === 13 && t.$button.trigger("click")
        }).focus(function () {
            t.$input.val("")
        }).blur(function () {
            t.$input.val() || (t.$input.val(t.input_label), t.$input.attr("style", t.input_muted_style))
        }), this.$input
    };
    t.prototype.setupLookupButton = function () {
        var t = this;
        return this.$button = n(this.button).length ? n(this.button).first() : n("<button />", {
            html: this.button_label,
            id: this.button_id,
            type: "button",
            class:'btn btn-primary'
        }).appendTo(this.$context).addClass(this.button_class).attr("onclick", "return false;").attr('style','margin-top:-3px;').submit(function () {
            return !1
        }), this.$button.click(function () {
            var n = t.$input.val();
            t.disableLookup();
            t.clearAll();
            t.lookupPostcode(n)
        }), this.$button
    };
    t.prototype.disableLookup = function (n) {
        n = n || this.button_disabled_message;
        this.$button.prop("disabled", !0).html(n)
    };
    t.prototype.enableLookup = function () {
        var n = this;
        n.lookup_interval === 0 ? n.$button.prop("disabled", !1).html(n.button_label) : setTimeout(function () {
            n.$button.prop("disabled", !1).html(n.button_label)
        }, n.lookup_interval)
    };
    t.prototype.clearAll = function () {
        this.setDropDown();
        this.setErrorMessage();
        this.setAddressFields()
    };
    t.prototype.removeAll = function () {
        this.$context = null;
        n.each([this.$input, this.$button, this.$dropdown, this.$error_message], function (n, t) {
            t && t.remove()
        })
    };
    t.prototype.lookupPostcode = function (t) {
        var i = this;
        if (!n.getAddress.validatePostcodeFormat(t)) return this.enableLookup(), i.setErrorMessage(this.error_message_postcode_invalid);
        n.getAddress.lookupPostcode(t, i.api_key, function (n) {
            if (i.enableLookup(), i.sort_numerically && (n.Addresses = f(n.Addresses)), i.setDropDown(n.Addresses, t), i.onLookupSuccess) i.onLookupSuccess(n)
        }, function (n) {
            n.status == 404 ? i.setErrorMessage(i.error_message_postcode_not_found) : i.setErrorMessage("Unable to connect to server");
            i.enableLookup();
            i.onLookupError && i.onLookupError()
        })
    };
    t.prototype.setDropDown = function (t, i) {
        var r = this,
            u, e, f;
        if (this.$dropdown && this.$dropdown.length && (this.$dropdown.remove(), delete this.$dropdown), t) {
            for (u = n("<select />", {
                id: r.dropdown_id
            }).addClass(r.dropdown_class), n("<option />", {
                value: "open",
                text: r.dropdown_select_message
            }).appendTo(u), e = t.length, f = 0; f < e; f += 1) {
                var o = t[f].split(","),
                    s = o.clean(!1),
                    h = s.join(",");
                n("<option />", {
                    value: f,
                    text: h
                }).appendTo(u)
            }
            return u.appendTo(r.$context).change(function () {
                var u = n(this).val();
                u >= 0 && (r.setAddressFields(t[u], i), r.onAddressSelected && r.onAddressSelected.call(this, t[u]))
            }), r.$dropdown = u, u
        }
    };
    t.prototype.setErrorMessage = function (t) {
        if (this.$error_message && this.$error_message.length && (this.$error_message.remove(), delete this.$error_message), t) return this.$error_message = n("<p />", {
            html: t,
            id: this.error_message_id
        }).addClass(this.error_message_class).appendTo(this.$context), this.$error_message
    };
    t.prototype.setAddressFields = function (n, t) {
        var r, i;
        for (r in this.$output_fields) this.$output_fields[r].val("");
        n && (i = n.split(","), this.$output_fields.line_1 && this.$output_fields.line_1.val(i[0].trim() || ""), this.$output_fields.line_2 && this.$output_fields.line_2.val(i[1].trim() || ""), i[2].trim() && i[3].trim() && this.$output_fields.line_3 ? this.$output_fields.line_3.val(i[2].trim() + ", " + i[3].trim()) : i[2].trim() && this.$output_fields.line_3 ? this.$output_fields.line_3.val(i[2].trim() || "") : i[3].trim() && this.$output_fields.line_3 && this.$output_fields.line_3.val(i[3].trim() || ""), i[4].trim() && i[5].trim() && this.$output_fields.post_town ? this.$output_fields.post_town.val(i[4].trim() + ", " + i[5].trim()) : i[5].trim() && this.$output_fields.post_town ? this.$output_fields.post_town.val(i[5].trim() || "") : i[4].trim() && this.$output_fields.post_town && this.$output_fields.post_town.val(i[4].trim() || ""), this.$output_fields.county && this.$output_fields.county.val(i[6].trim() || ""), t && (t = t.toUpperCase().trim()), this.$output_fields.postcode && this.$output_fields.postcode.val(t || ""))
    };
    n.getAddress = {
        defaults: function () {
            return u
        },
        setup: function (n) {
            r = new t(n);
            i.push(r)
        },
        validatePostcodeFormat: function (n) {
            return !!n.match(/^[a-zA-Z0-9]{1,4}\s?\d[a-zA-Z]{2}$/)
        },
        lookupPostcode: function (t, i, r, f) {
            var o = u.api_endpoint,
                s = [o, t].join("/"),
                e = {
                    url: s,
                    data: {
                        "api-key": i
                    },
                    dataType: "json",
                    timeout: 3e5,
                    success: r
                };
            f && (e.error = f);
            n.ajax(e)
        },
        clearAll: function () {
            for (var t = i.length, n = 0; n < t; n += 1) i[n].removeAll()
        }
    };
    n.fn.getAddress = function (u) {
        if (u) {
            var f = new t(u);
            i.push(f);
            f.setupPostcodeInput(n(this))
        } else r.setupPostcodeInput(n(this));
        return this
    }
})(jQuery);