!function (t) {
    "use strict";
    "object" == typeof module && "object" == typeof module.exports ? t(require("jquery"), window, document) : t(jQuery, window, document)
}(function (t, e, i, n) {
    "use strict";
    if (!e.history.pushState)return t.fn.smoothState = function () {
        return this
    }, void(t.fn.smoothState.options = {});
    if (!t.fn.smoothState) {
        var r = t("html, body"), s = e.console, o = {
            debug: !1,
            anchors: "a",
            hrefRegex: "",
            forms: "form",
            allowFormCaching: !1,
            repeatDelay: 500,
            blacklist: ".no-smoothState",
            prefetch: !1,
            prefetchOn: "mouseover touchstart",
            prefetchBlacklist: ".no-prefetch",
            cacheLength: 0,
            loadingClass: "is-loading",
            scroll: !0,
            alterRequest: function (t) {
                return t
            },
            alterChangeState: function (t, e, i) {
                return t
            },
            onBefore: function (t, e) {
            },
            onStart: {
                duration: 0, render: function (t) {
                }
            },
            onProgress: {
                duration: 0, render: function (t) {
                }
            },
            onReady: {
                duration: 0, render: function (t, e) {
                    t.html(e)
                }
            },
            onAfter: function (t, e) {
            }
        }, a = {
            isExternal: function (t) {
                var i = t.match(/^([^:\/?#]+:)?(?:\/\/([^\/?#]*))?([^?#]+)?(\?[^#]*)?(#.*)?/);
                return "string" == typeof i[1] && i[1].length > 0 && i[1].toLowerCase() !== e.location.protocol || "string" == typeof i[2] && i[2].length > 0 && i[2].replace(new RegExp(":(" + {
                            "http:": 80,
                            "https:": 443
                        }[e.location.protocol] + ")?$"), "") !== e.location.host
            }, stripHash: function (t) {
                return t.replace(/#.*/, "")
            }, isHash: function (t, i) {
                i = i || e.location.href;
                var n = t.indexOf("#") > -1, r = a.stripHash(t) === a.stripHash(i);
                return n && r
            }, translate: function (e) {
                var i = {dataType: "html", type: "GET"};
                return e = "string" == typeof e ? t.extend({}, i, {url: e}) : t.extend({}, i, e)
            }, shouldLoadAnchor: function (t, e, i) {
                var r = t.prop("href");
                return !(a.isExternal(r) || a.isHash(r) || t.is(e) || t.prop("target") || typeof i !== n && "" !== i && t.prop("href").search(i) === -1)
            }, clearIfOverCapacity: function (t, e) {
                return Object.keys || (Object.keys = function (t) {
                    var e, i = [];
                    for (e in t)Object.prototype.hasOwnProperty.call(t, e) && i.push(e);
                    return i
                }), Object.keys(t).length > e && (t = {}), t
            }, storePageIn: function (e, i, n, r, s) {
                var o = t("<html></html>").append(t(n));
                return e[i] = {
                    status: "loaded",
                    title: o.find("title").first().text(),
                    html: o.find("#" + r),
                    doc: n,
                    state: s
                }, e
            }, triggerAllAnimationEndEvent: function (e, i) {
                i = " " + i || "";
                var n = 0, r = "animationstart webkitAnimationStart oanimationstart MSAnimationStart", s = "animationend webkitAnimationEnd oanimationend MSAnimationEnd", o = "allanimationend", l = function (i) {
                    t(i.delegateTarget).is(e) && (i.stopPropagation(), n++)
                }, u = function (i) {
                    t(i.delegateTarget).is(e) && (i.stopPropagation(), n--, 0 === n && e.trigger(o))
                };
                e.on(r, l), e.on(s, u), e.on("allanimationend" + i, function () {
                    n = 0, a.redraw(e)
                })
            }, redraw: function (t) {
                t.height()
            }
        }, l = function (i) {
            if (null !== i.state) {
                var n = e.location.href, r = t("#" + i.state.id), s = r.data("smoothState"), o = s.href !== n && !a.isHash(n, s.href), l = i.state !== s.cache[s.href].state;
                (o || l) && (l && s.clear(s.href), s.load(n, !1))
            }
        }, u = function (o, l) {
            var u = t(o), h = u.prop("id"), c = null, f = !1, p = {}, d = {}, m = e.location.href, R = function (t) {
                t = t || !1, t && p.hasOwnProperty(t) ? delete p[t] : p = {}, u.data("smoothState").cache = p
            }, g = function (e, i) {
                i = i || t.noop;
                var n = a.translate(e);
                if (p = a.clearIfOverCapacity(p, l.cacheLength), !p.hasOwnProperty(n.url) || "undefined" != typeof n.data) {
                    p[n.url] = {status: "fetching"};
                    var r = t.ajax(n);
                    r.done(function (t) {
                        a.storePageIn(p, n.url, t, h), u.data("smoothState").cache = p
                    }), r.fail(function () {
                        p[n.url].status = "error"
                    }), i && r.always(i)
                }
            }, _ = function () {
                if (c) {
                    var e = t(c, u);
                    if (e.length) {
                        var i = e.offset().top;
                        r.scrollTop(i)
                    }
                    c = null
                }
            }, v = function (n) {
                var o = "#" + h, a = p[n] ? t(p[n].html.html()) : null;
                a.length ? (i.title = p[n].title, u.data("smoothState").href = n, l.loadingClass && r.removeClass(l.loadingClass), l.onReady.render(u, a), u.one("ss.onReadyEnd", function () {
                    f = !1, l.onAfter(u, a), l.scroll && _(), k(u)
                }), e.setTimeout(function () {
                    u.trigger("ss.onReadyEnd")
                }, l.onReady.duration)) : !a && l.debug && s ? s.warn("No element with an id of " + o + " in response from " + n + " in " + p) : e.location = n
            }, y = function (t, i, n) {
                var o = a.translate(t);
                "undefined" == typeof i && (i = !0), "undefined" == typeof n && (n = !0);
                var c = !1, f = !1, m = {
                    loaded: function () {
                        var t = c ? "ss.onProgressEnd" : "ss.onStartEnd";
                        f && c ? f && v(o.url) : u.one(t, function () {
                            v(o.url), n || R(o.url)
                        }), i && (d = l.alterChangeState({id: h}, p[o.url].title, o.url), p[o.url].state = d, e.history.pushState(d, p[o.url].title, o.url)), f && !n && R(o.url)
                    }, fetching: function () {
                        c || (c = !0, u.one("ss.onStartEnd", function () {
                            l.loadingClass && r.addClass(l.loadingClass), l.onProgress.render(u), e.setTimeout(function () {
                                u.trigger("ss.onProgressEnd"), f = !0
                            }, l.onProgress.duration)
                        })), e.setTimeout(function () {
                            p.hasOwnProperty(o.url) && m[p[o.url].status]()
                        }, 10)
                    }, error: function () {
                        l.debug && s ? s.log("There was an error loading: " + o.url) : e.location = o.url
                    }
                };
                p.hasOwnProperty(o.url) || g(o), l.onStart.render(u), e.setTimeout(function () {
                    l.scroll && r.scrollTop(0), u.trigger("ss.onStartEnd")
                }, l.onStart.duration), m[p[o.url].status]()
            }, x = function (e) {
                var i, n = t(e.currentTarget);
                a.shouldLoadAnchor(n, l.blacklist, l.hrefRegex) && !f && (e.stopPropagation(), i = a.translate(n.prop("href")), i = l.alterRequest(i), g(i))
            }, w = function (e) {
                var i = t(e.currentTarget);
                if (!e.metaKey && !e.ctrlKey && a.shouldLoadAnchor(i, l.blacklist, l.hrefRegex) && (e.stopPropagation(), e.preventDefault(), !z())) {
                    P();
                    var n = a.translate(i.prop("href"));
                    f = !0, c = i.prop("hash"), n = l.alterRequest(n), l.onBefore(i, u), y(n)
                }
            }, b = function (e) {
                var i = t(e.currentTarget);
                if (!i.is(l.blacklist) && (e.preventDefault(), e.stopPropagation(), !z())) {
                    P();
                    var r = {url: i.prop("action"), data: i.serialize(), type: i.prop("method")};
                    f = !0, r = l.alterRequest(r), "get" === r.type.toLowerCase() && (r.url = r.url + "?" + r.data), l.onBefore(i, u), y(r, n, l.allowFormCaching)
                }
            }, T = 0, z = function () {
                var t = null === l.repeatDelay, e = parseInt(Date.now()) > T;
                return !(t || e)
            }, P = function () {
                T = parseInt(Date.now()) + parseInt(l.repeatDelay)
            }, k = function (t) {
                l.anchors && l.prefetch && t.find(l.anchors).not(l.prefetchBlacklist).on(l.prefetchOn, null, x)
            }, O = function (t) {
                l.anchors && (t.on("click", l.anchors, w), k(t)), l.forms && t.on("submit", l.forms, b)
            }, C = function () {
                var t = u.prop("class");
                u.removeClass(t), a.redraw(u), u.addClass(t)
            };
            return l = t.extend({}, t.fn.smoothState.options, l), null === e.history.state ? (d = l.alterChangeState({id: h}, i.title, m), e.history.replaceState(d, i.title, m)) : d = {}, a.storePageIn(p, m, i.documentElement.outerHTML, h, d), a.triggerAllAnimationEndEvent(u, "ss.onStartEnd ss.onProgressEnd ss.onEndEnd"), O(u), {
                href: m,
                cache: p,
                clear: R,
                load: y,
                fetch: g,
                restartCSSAnimations: C
            }
        }, h = function (e) {
            return this.each(function () {
                var i = this.tagName.toLowerCase();
                this.id && "body" !== i && "html" !== i && !t.data(this, "smoothState") ? t.data(this, "smoothState", new u(this, e)) : !this.id && s ? s.warn("Every smoothState container needs an id but the following one does not have one:", this) : "body" !== i && "html" !== i || !s || s.warn("The smoothstate container cannot be the " + this.tagName + " tag")
            })
        };
        e.onpopstate = l, t.smoothStateUtility = a, t.fn.smoothState = h, t.fn.smoothState.options = o
    }
}), !function (t, e, i) {
    function n(t, e) {
        return typeof t === e
    }

    function r() {
        var t, e, i, r, s, o, a;
        for (var l in h)if (h.hasOwnProperty(l)) {
            if (t = [], e = h[l], e.name && (t.push(e.name.toLowerCase()), e.options && e.options.aliases && e.options.aliases.length))for (i = 0; i < e.options.aliases.length; i++)t.push(e.options.aliases[i].toLowerCase());
            for (r = n(e.fn, "function") ? e.fn() : e.fn, s = 0; s < t.length; s++)o = t[s], a = o.split("."), 1 === a.length ? f[a[0]] = r : (!f[a[0]] || f[a[0]] instanceof Boolean || (f[a[0]] = new Boolean(f[a[0]])), f[a[0]][a[1]] = r), u.push((r ? "" : "no-") + a.join("-"))
        }
    }

    function s(t) {
        var e = p.className, i = f._config.classPrefix || "";
        if (d && (e = e.baseVal), f._config.enableJSClass) {
            var n = new RegExp("(^|\\s)" + i + "no-js(\\s|$)");
            e = e.replace(n, "$1" + i + "js$2")
        }
        f._config.enableClasses && (e += " " + i + t.join(" " + i), d ? p.className.baseVal = e : p.className = e)
    }

    function o() {
        return "function" != typeof e.createElement ? e.createElement(arguments[0]) : d ? e.createElementNS.call(e, "http://www.w3.org/2000/svg", arguments[0]) : e.createElement.apply(e, arguments)
    }

    function a() {
        var t = e.body;
        return t || (t = o(d ? "svg" : "body"), t.fake = !0), t
    }

    function l(t, i, n, r) {
        var s, l, u, h, c = "modernizr", f = o("div"), d = a();
        if (parseInt(n, 10))for (; n--;)u = o("div"), u.id = r ? r[n] : c + (n + 1), f.appendChild(u);
        return s = o("style"), s.type = "text/css", s.id = "s" + c, (d.fake ? d : f).appendChild(s), d.appendChild(f), s.styleSheet ? s.styleSheet.cssText = t : s.appendChild(e.createTextNode(t)), f.id = c, d.fake && (d.style.background = "", d.style.overflow = "hidden", h = p.style.overflow, p.style.overflow = "hidden", p.appendChild(d)), l = i(f, t), d.fake ? (d.parentNode.removeChild(d), p.style.overflow = h, p.offsetHeight) : f.parentNode.removeChild(f), !!l
    }

    var u = [], h = [], c = {
        _version: "3.3.1",
        _config: {classPrefix: "", enableClasses: !0, enableJSClass: !0, usePrefixes: !0},
        _q: [],
        on: function (t, e) {
            var i = this;
            setTimeout(function () {
                e(i[t])
            }, 0)
        },
        addTest: function (t, e, i) {
            h.push({name: t, fn: e, options: i})
        },
        addAsyncTest: function (t) {
            h.push({name: null, fn: t})
        }
    }, f = function () {
    };
    f.prototype = c, f = new f;
    var p = e.documentElement, d = "svg" === p.nodeName.toLowerCase(), m = function () {
        var e = t.matchMedia || t.msMatchMedia;
        return e ? function (t) {
            var i = e(t);
            return i && i.matches || !1
        } : function (e) {
            var i = !1;
            return l("@media " + e + " { #modernizr { position: absolute; } }", function (e) {
                i = "absolute" == (t.getComputedStyle ? t.getComputedStyle(e, null) : e.currentStyle).position
            }), i
        }
    }();
    c.mq = m, r(), s(u), delete c.addTest, delete c.addAsyncTest;
    for (var R = 0; R < f._q.length; R++)f._q[R]();
    t.Modernizr = f
}(window, document), !function (t, e) {
    "use strict";
    var i = {}, n = t.GreenSockGlobals = t.GreenSockGlobals || t;
    if (!n.TweenLite) {
        var r, s, o, a, l, u = function (t) {
            var e, i = t.split("."), r = n;
            for (e = 0; e < i.length; e++)r[i[e]] = r = r[i[e]] || {};
            return r
        }, h = u("com.greensock"), c = 1e-10, f = function (t) {
            var e, i = [], n = t.length;
            for (e = 0; e !== n; i.push(t[e++]));
            return i
        }, p = function () {
        }, d = function () {
            var t = Object.prototype.toString, e = t.call([]);
            return function (i) {
                return null != i && (i instanceof Array || "object" == typeof i && !!i.push && t.call(i) === e)
            }
        }(), m = {}, R = function (r, s, o, a) {
            this.sc = m[r] ? m[r].sc : [], m[r] = this, this.gsClass = null, this.func = o;
            var l = [];
            this.check = function (h) {
                for (var c, f, p, d, g, _ = s.length, v = _; --_ > -1;)(c = m[s[_]] || new R(s[_], [])).gsClass ? (l[_] = c.gsClass, v--) : h && c.sc.push(this);
                if (0 === v && o) {
                    if (f = ("com.greensock." + r).split("."), p = f.pop(), d = u(f.join("."))[p] = this.gsClass = o.apply(o, l), a)if (n[p] = d, g = "undefined" != typeof module && module.exports, !g && "function" == typeof define && define.amd)define((t.GreenSockAMDPath ? t.GreenSockAMDPath + "/" : "") + r.split(".").pop(), [], function () {
                        return d
                    }); else if (g)if (r === e) {
                        module.exports = i[e] = d;
                        for (_ in i)d[_] = i[_]
                    } else i[e] && (i[e][p] = d);
                    for (_ = 0; _ < this.sc.length; _++)this.sc[_].check()
                }
            }, this.check(!0)
        }, g = t._gsDefine = function (t, e, i, n) {
            return new R(t, e, i, n)
        }, _ = h._class = function (t, e, i) {
            return e = e || function () {
                }, g(t, [], function () {
                return e
            }, i), e
        };
        g.globals = n;
        var v = [0, 0, 1, 1], y = [], x = _("easing.Ease", function (t, e, i, n) {
            this._func = t, this._type = i || 0, this._power = n || 0, this._params = e ? v.concat(e) : v
        }, !0), w = x.map = {}, b = x.register = function (t, e, i, n) {
            for (var r, s, o, a, l = e.split(","), u = l.length, c = (i || "easeIn,easeOut,easeInOut").split(","); --u > -1;)for (s = l[u], r = n ? _("easing." + s, null, !0) : h.easing[s] || {}, o = c.length; --o > -1;)a = c[o], w[s + "." + a] = w[a + s] = r[a] = t.getRatio ? t : t[a] || new t
        };
        for (o = x.prototype, o._calcEnd = !1, o.getRatio = function (t) {
            if (this._func)return this._params[0] = t, this._func.apply(null, this._params);
            var e = this._type, i = this._power, n = 1 === e ? 1 - t : 2 === e ? t : .5 > t ? 2 * t : 2 * (1 - t);
            return 1 === i ? n *= n : 2 === i ? n *= n * n : 3 === i ? n *= n * n * n : 4 === i && (n *= n * n * n * n), 1 === e ? 1 - n : 2 === e ? n : .5 > t ? n / 2 : 1 - n / 2
        }, r = ["Linear", "Quad", "Cubic", "Quart", "Quint,Strong"], s = r.length; --s > -1;)o = r[s] + ",Power" + s, b(new x(null, null, 1, s), o, "easeOut", !0), b(new x(null, null, 2, s), o, "easeIn" + (0 === s ? ",easeNone" : "")), b(new x(null, null, 3, s), o, "easeInOut");
        w.linear = h.easing.Linear.easeIn, w.swing = h.easing.Quad.easeInOut;
        var T = _("events.EventDispatcher", function (t) {
            this._listeners = {}, this._eventTarget = t || this
        });
        o = T.prototype, o.addEventListener = function (t, e, i, n, r) {
            r = r || 0;
            var s, o, u = this._listeners[t], h = 0;
            for (this !== a || l || a.wake(), null == u && (this._listeners[t] = u = []), o = u.length; --o > -1;)s = u[o], s.c === e && s.s === i ? u.splice(o, 1) : 0 === h && s.pr < r && (h = o + 1);
            u.splice(h, 0, {c: e, s: i, up: n, pr: r})
        }, o.removeEventListener = function (t, e) {
            var i, n = this._listeners[t];
            if (n)for (i = n.length; --i > -1;)if (n[i].c === e)return void n.splice(i, 1)
        }, o.dispatchEvent = function (t) {
            var e, i, n, r = this._listeners[t];
            if (r)for (e = r.length, i = this._eventTarget; --e > -1;)n = r[e], n && (n.up ? n.c.call(n.s || i, {
                type: t,
                target: i
            }) : n.c.call(n.s || i))
        };
        var z = t.requestAnimationFrame, P = t.cancelAnimationFrame, k = Date.now || function () {
                return (new Date).getTime()
            }, O = k();
        for (r = ["ms", "moz", "webkit", "o"], s = r.length; --s > -1 && !z;)z = t[r[s] + "RequestAnimationFrame"], P = t[r[s] + "CancelAnimationFrame"] || t[r[s] + "CancelRequestAnimationFrame"];
        _("Ticker", function (t, e) {
            var i, n, r, s, o, u = this, h = k(), f = !(e === !1 || !z) && "auto", d = 500, m = 33, R = "tick", g = function (t) {
                var e, a, l = k() - O;
                l > d && (h += l - m), O += l, u.time = (O - h) / 1e3, e = u.time - o, (!i || e > 0 || t === !0) && (u.frame++, o += e + (e >= s ? .004 : s - e), a = !0), t !== !0 && (r = n(g)), a && u.dispatchEvent(R)
            };
            T.call(u), u.time = u.frame = 0, u.tick = function () {
                g(!0)
            }, u.lagSmoothing = function (t, e) {
                d = t || 1 / c, m = Math.min(e, d, 0)
            }, u.sleep = function () {
                null != r && (f && P ? P(r) : clearTimeout(r), n = p, r = null, u === a && (l = !1))
            }, u.wake = function (t) {
                null !== r ? u.sleep() : t ? h += -O + (O = k()) : u.frame > 10 && (O = k() - d + 5), n = 0 === i ? p : f && z ? z : function (t) {
                    return setTimeout(t, 1e3 * (o - u.time) + 1 | 0)
                }, u === a && (l = !0), g(2)
            }, u.fps = function (t) {
                return arguments.length ? (i = t, s = 1 / (i || 60), o = this.time + s, void u.wake()) : i
            }, u.useRAF = function (t) {
                return arguments.length ? (u.sleep(), f = t, void u.fps(i)) : f
            }, u.fps(t), setTimeout(function () {
                "auto" === f && u.frame < 5 && "hidden" !== document.visibilityState && u.useRAF(!1)
            }, 1500)
        }), o = h.Ticker.prototype = new h.events.EventDispatcher, o.constructor = h.Ticker;
        var C = _("core.Animation", function (t, e) {
            if (this.vars = e = e || {}, this._duration = this._totalDuration = t || 0, this._delay = Number(e.delay) || 0, this._timeScale = 1, this._active = e.immediateRender === !0, this.data = e.data, this._reversed = e.reversed === !0, W) {
                l || a.wake();
                var i = this.vars.useFrames ? H : W;
                i.add(this, i._time), this.vars.paused && this.paused(!0)
            }
        });
        a = C.ticker = new h.Ticker, o = C.prototype, o._dirty = o._gc = o._initted = o._paused = !1, o._totalTime = o._time = 0, o._rawPrevTime = -1, o._next = o._last = o._onUpdate = o._timeline = o.timeline = null, o._paused = !1;
        var S = function () {
            l && k() - O > 2e3 && a.wake(), setTimeout(S, 2e3)
        };
        S(), o.play = function (t, e) {
            return null != t && this.seek(t, e), this.reversed(!1).paused(!1)
        }, o.pause = function (t, e) {
            return null != t && this.seek(t, e), this.paused(!0)
        }, o.resume = function (t, e) {
            return null != t && this.seek(t, e), this.paused(!1)
        }, o.seek = function (t, e) {
            return this.totalTime(Number(t), e !== !1)
        }, o.restart = function (t, e) {
            return this.reversed(!1).paused(!1).totalTime(t ? -this._delay : 0, e !== !1, !0)
        }, o.reverse = function (t, e) {
            return null != t && this.seek(t || this.totalDuration(), e), this.reversed(!0).paused(!1)
        }, o.render = function (t, e, i) {
        }, o.invalidate = function () {
            return this._time = this._totalTime = 0, this._initted = this._gc = !1, this._rawPrevTime = -1, (this._gc || !this.timeline) && this._enabled(!0), this
        }, o.isActive = function () {
            var t, e = this._timeline, i = this._startTime;
            return !e || !this._gc && !this._paused && e.isActive() && (t = e.rawTime()) >= i && t < i + this.totalDuration() / this._timeScale
        }, o._enabled = function (t, e) {
            return l || a.wake(), this._gc = !t, this._active = this.isActive(), e !== !0 && (t && !this.timeline ? this._timeline.add(this, this._startTime - this._delay) : !t && this.timeline && this._timeline._remove(this, !0)), !1
        }, o._kill = function (t, e) {
            return this._enabled(!1, !1)
        }, o.kill = function (t, e) {
            return this._kill(t, e), this
        }, o._uncache = function (t) {
            for (var e = t ? this : this.timeline; e;)e._dirty = !0, e = e.timeline;
            return this
        }, o._swapSelfInParams = function (t) {
            for (var e = t.length, i = t.concat(); --e > -1;)"{self}" === t[e] && (i[e] = this);
            return i
        }, o._callback = function (t) {
            var e = this.vars;
            e[t].apply(e[t + "Scope"] || e.callbackScope || this, e[t + "Params"] || y)
        }, o.eventCallback = function (t, e, i, n) {
            if ("on" === (t || "").substr(0, 2)) {
                var r = this.vars;
                if (1 === arguments.length)return r[t];
                null == e ? delete r[t] : (r[t] = e, r[t + "Params"] = d(i) && -1 !== i.join("").indexOf("{self}") ? this._swapSelfInParams(i) : i, r[t + "Scope"] = n), "onUpdate" === t && (this._onUpdate = e)
            }
            return this
        }, o.delay = function (t) {
            return arguments.length ? (this._timeline.smoothChildTiming && this.startTime(this._startTime + t - this._delay), this._delay = t, this) : this._delay
        }, o.duration = function (t) {
            return arguments.length ? (this._duration = this._totalDuration = t, this._uncache(!0), this._timeline.smoothChildTiming && this._time > 0 && this._time < this._duration && 0 !== t && this.totalTime(this._totalTime * (t / this._duration), !0), this) : (this._dirty = !1, this._duration)
        }, o.totalDuration = function (t) {
            return this._dirty = !1, arguments.length ? this.duration(t) : this._totalDuration
        }, o.time = function (t, e) {
            return arguments.length ? (this._dirty && this.totalDuration(), this.totalTime(t > this._duration ? this._duration : t, e)) : this._time
        }, o.totalTime = function (t, e, i) {
            if (l || a.wake(), !arguments.length)return this._totalTime;
            if (this._timeline) {
                if (0 > t && !i && (t += this.totalDuration()), this._timeline.smoothChildTiming) {
                    this._dirty && this.totalDuration();
                    var n = this._totalDuration, r = this._timeline;
                    if (t > n && !i && (t = n), this._startTime = (this._paused ? this._pauseTime : r._time) - (this._reversed ? n - t : t) / this._timeScale, r._dirty || this._uncache(!1), r._timeline)for (; r._timeline;)r._timeline._time !== (r._startTime + r._totalTime) / r._timeScale && r.totalTime(r._totalTime, !0), r = r._timeline
                }
                this._gc && this._enabled(!0, !1), (this._totalTime !== t || 0 === this._duration) && (F.length && Z(), this.render(t, e, !1), F.length && Z())
            }
            return this
        }, o.progress = o.totalProgress = function (t, e) {
            var i = this.duration();
            return arguments.length ? this.totalTime(i * t, e) : i ? this._time / i : this.ratio
        }, o.startTime = function (t) {
            return arguments.length ? (t !== this._startTime && (this._startTime = t, this.timeline && this.timeline._sortChildren && this.timeline.add(this, t - this._delay)), this) : this._startTime
        }, o.endTime = function (t) {
            return this._startTime + (0 != t ? this.totalDuration() : this.duration()) / this._timeScale
        }, o.timeScale = function (t) {
            if (!arguments.length)return this._timeScale;
            if (t = t || c, this._timeline && this._timeline.smoothChildTiming) {
                var e = this._pauseTime, i = e || 0 === e ? e : this._timeline.totalTime();
                this._startTime = i - (i - this._startTime) * this._timeScale / t
            }
            return this._timeScale = t, this._uncache(!1)
        }, o.reversed = function (t) {
            return arguments.length ? (t != this._reversed && (this._reversed = t, this.totalTime(this._timeline && !this._timeline.smoothChildTiming ? this.totalDuration() - this._totalTime : this._totalTime, !0)), this) : this._reversed
        }, o.paused = function (t) {
            if (!arguments.length)return this._paused;
            var e, i, n = this._timeline;
            return t != this._paused && n && (l || t || a.wake(), e = n.rawTime(), i = e - this._pauseTime, !t && n.smoothChildTiming && (this._startTime += i, this._uncache(!1)), this._pauseTime = t ? e : null, this._paused = t, this._active = this.isActive(), !t && 0 !== i && this._initted && this.duration() && (e = n.smoothChildTiming ? this._totalTime : (e - this._startTime) / this._timeScale, this.render(e, e === this._totalTime, !0))), this._gc && !t && this._enabled(!0, !1), this
        };
        var A = _("core.SimpleTimeline", function (t) {
            C.call(this, 0, t), this.autoRemoveChildren = this.smoothChildTiming = !0
        });
        o = A.prototype = new C, o.constructor = A, o.kill()._gc = !1, o._first = o._last = o._recent = null, o._sortChildren = !1, o.add = o.insert = function (t, e, i, n) {
            var r, s;
            if (t._startTime = Number(e || 0) + t._delay, t._paused && this !== t._timeline && (t._pauseTime = t._startTime + (this.rawTime() - t._startTime) / t._timeScale), t.timeline && t.timeline._remove(t, !0), t.timeline = t._timeline = this, t._gc && t._enabled(!0, !0), r = this._last, this._sortChildren)for (s = t._startTime; r && r._startTime > s;)r = r._prev;
            return r ? (t._next = r._next, r._next = t) : (t._next = this._first, this._first = t), t._next ? t._next._prev = t : this._last = t, t._prev = r, this._recent = t, this._timeline && this._uncache(!0), this
        }, o._remove = function (t, e) {
            return t.timeline === this && (e || t._enabled(!1, !0), t._prev ? t._prev._next = t._next : this._first === t && (this._first = t._next), t._next ? t._next._prev = t._prev : this._last === t && (this._last = t._prev), t._next = t._prev = t.timeline = null, t === this._recent && (this._recent = this._last), this._timeline && this._uncache(!0)), this
        }, o.render = function (t, e, i) {
            var n, r = this._first;
            for (this._totalTime = this._time = this._rawPrevTime = t; r;)n = r._next, (r._active || t >= r._startTime && !r._paused) && (r._reversed ? r.render((r._dirty ? r.totalDuration() : r._totalDuration) - (t - r._startTime) * r._timeScale, e, i) : r.render((t - r._startTime) * r._timeScale, e, i)), r = n
        }, o.rawTime = function () {
            return l || a.wake(), this._totalTime
        };
        var M = _("TweenLite", function (e, i, n) {
            if (C.call(this, i, n), this.render = M.prototype.render, null == e)throw"Cannot tween a null target.";
            this.target = e = "string" != typeof e ? e : M.selector(e) || e;
            var r, s, o, a = e.jquery || e.length && e !== t && e[0] && (e[0] === t || e[0].nodeType && e[0].style && !e.nodeType), l = this.vars.overwrite;
            if (this._overwrite = l = null == l ? G[M.defaultOverwrite] : "number" == typeof l ? l >> 0 : G[l], (a || e instanceof Array || e.push && d(e)) && "number" != typeof e[0])for (this._targets = o = f(e), this._propLookup = [], this._siblings = [], r = 0; r < o.length; r++)s = o[r], s ? "string" != typeof s ? s.length && s !== t && s[0] && (s[0] === t || s[0].nodeType && s[0].style && !s.nodeType) ? (o.splice(r--, 1), this._targets = o = o.concat(f(s))) : (this._siblings[r] = $(s, this, !1), 1 === l && this._siblings[r].length > 1 && J(s, this, null, 1, this._siblings[r])) : (s = o[r--] = M.selector(s), "string" == typeof s && o.splice(r + 1, 1)) : o.splice(r--, 1); else this._propLookup = {}, this._siblings = $(e, this, !1), 1 === l && this._siblings.length > 1 && J(e, this, null, 1, this._siblings);
            (this.vars.immediateRender || 0 === i && 0 === this._delay && this.vars.immediateRender !== !1) && (this._time = -c, this.render(Math.min(0, -this._delay)))
        }, !0), E = function (e) {
            return e && e.length && e !== t && e[0] && (e[0] === t || e[0].nodeType && e[0].style && !e.nodeType)
        }, N = function (t, e) {
            var i, n = {};
            for (i in t)U[i] || i in e && "transform" !== i && "x" !== i && "y" !== i && "width" !== i && "height" !== i && "className" !== i && "border" !== i || !(!V[i] || V[i] && V[i]._autoCSS) || (n[i] = t[i], delete t[i]);
            t.css = n
        };
        o = M.prototype = new C, o.constructor = M, o.kill()._gc = !1, o.ratio = 0, o._firstPT = o._targets = o._overwrittenProps = o._startAt = null, o._notifyPluginsOfEnabled = o._lazy = !1, M.version = "1.18.5", M.defaultEase = o._ease = new x(null, null, 1, 1), M.defaultOverwrite = "auto", M.ticker = a, M.autoSleep = 120, M.lagSmoothing = function (t, e) {
            a.lagSmoothing(t, e)
        }, M.selector = t.$ || t.jQuery || function (e) {
                var i = t.$ || t.jQuery;
                return i ? (M.selector = i, i(e)) : "undefined" == typeof document ? e : document.querySelectorAll ? document.querySelectorAll(e) : document.getElementById("#" === e.charAt(0) ? e.substr(1) : e)
            };
        var F = [], I = {}, L = /(?:(-|-=|\+=)?\d*\.?\d*(?:e[\-+]?\d+)?)[0-9]/gi, X = function (t) {
            for (var e, i = this._firstPT, n = 1e-6; i;)e = i.blob ? t ? this.join("") : this.start : i.c * t + i.s, i.r ? e = Math.round(e) : n > e && e > -n && (e = 0), i.f ? i.fp ? i.t[i.p](i.fp, e) : i.t[i.p](e) : i.t[i.p] = e, i = i._next
        }, j = function (t, e, i, n) {
            var r, s, o, a, l, u, h, c = [t, e], f = 0, p = "", d = 0;
            for (c.start = t, i && (i(c), t = c[0], e = c[1]), c.length = 0, r = t.match(L) || [], s = e.match(L) || [], n && (n._next = null, n.blob = 1, c._firstPT = n), l = s.length, a = 0; l > a; a++)h = s[a], u = e.substr(f, e.indexOf(h, f) - f), p += u || !a ? u : ",", f += u.length, d ? d = (d + 1) % 5 : "rgba(" === u.substr(-5) && (d = 1), h === r[a] || r.length <= a ? p += h : (p && (c.push(p), p = ""), o = parseFloat(r[a]), c.push(o), c._firstPT = {
                _next: c._firstPT,
                t: c,
                p: c.length - 1,
                s: o,
                c: ("=" === h.charAt(1) ? parseInt(h.charAt(0) + "1", 10) * parseFloat(h.substr(2)) : parseFloat(h) - o) || 0,
                f: 0,
                r: d && 4 > d
            }), f += h.length;
            return p += e.substr(f), p && c.push(p), c.setRatio = X, c
        }, D = function (t, e, i, n, r, s, o, a) {
            var l, u, h = "get" === i ? t[e] : i, c = typeof t[e], f = "string" == typeof n && "=" === n.charAt(1), p = {
                t: t,
                p: e,
                s: h,
                f: "function" === c,
                pg: 0,
                n: r || e,
                r: s,
                pr: 0,
                c: f ? parseInt(n.charAt(0) + "1", 10) * parseFloat(n.substr(2)) : parseFloat(n) - h || 0
            };
            return "number" !== c && ("function" === c && "get" === i && (u = e.indexOf("set") || "function" != typeof t["get" + e.substr(3)] ? e : "get" + e.substr(3), p.s = h = o ? t[u](o) : t[u]()), "string" == typeof h && (o || isNaN(h)) ? (p.fp = o, l = j(h, n, a || M.defaultStringFilter, p), p = {
                t: l,
                p: "setRatio",
                s: 0,
                c: 1,
                f: 2,
                pg: 0,
                n: r || e,
                pr: 0
            }) : f || (p.s = parseFloat(h), p.c = parseFloat(n) - p.s || 0)), p.c ? ((p._next = this._firstPT) && (p._next._prev = p), this._firstPT = p, p) : void 0
        }, Y = M._internals = {
            isArray: d,
            isSelector: E,
            lazyTweens: F,
            blobDif: j
        }, V = M._plugins = {}, B = Y.tweenLookup = {}, q = 0, U = Y.reservedProps = {
            ease: 1,
            delay: 1,
            overwrite: 1,
            onComplete: 1,
            onCompleteParams: 1,
            onCompleteScope: 1,
            useFrames: 1,
            runBackwards: 1,
            startAt: 1,
            onUpdate: 1,
            onUpdateParams: 1,
            onUpdateScope: 1,
            onStart: 1,
            onStartParams: 1,
            onStartScope: 1,
            onReverseComplete: 1,
            onReverseCompleteParams: 1,
            onReverseCompleteScope: 1,
            onRepeat: 1,
            onRepeatParams: 1,
            onRepeatScope: 1,
            easeParams: 1,
            yoyo: 1,
            immediateRender: 1,
            repeat: 1,
            repeatDelay: 1,
            data: 1,
            paused: 1,
            reversed: 1,
            autoCSS: 1,
            lazy: 1,
            onOverwrite: 1,
            callbackScope: 1,
            stringFilter: 1,
            id: 1
        }, G = {
            none: 0,
            all: 1,
            auto: 2,
            concurrent: 3,
            allOnStart: 4,
            preexisting: 5,
            "true": 1,
            "false": 0
        }, H = C._rootFramesTimeline = new A, W = C._rootTimeline = new A, Q = 30, Z = Y.lazyRender = function () {
            var t, e = F.length;
            for (I = {}; --e > -1;)t = F[e], t && t._lazy !== !1 && (t.render(t._lazy[0], t._lazy[1], !0), t._lazy = !1);
            F.length = 0
        };
        W._startTime = a.time, H._startTime = a.frame, W._active = H._active = !0, setTimeout(Z, 1), C._updateRoot = M.render = function () {
            var t, e, i;
            if (F.length && Z(), W.render((a.time - W._startTime) * W._timeScale, !1, !1), H.render((a.frame - H._startTime) * H._timeScale, !1, !1), F.length && Z(), a.frame >= Q) {
                Q = a.frame + (parseInt(M.autoSleep, 10) || 120);
                for (i in B) {
                    for (e = B[i].tweens, t = e.length; --t > -1;)e[t]._gc && e.splice(t, 1);
                    0 === e.length && delete B[i]
                }
                if (i = W._first, (!i || i._paused) && M.autoSleep && !H._first && 1 === a._listeners.tick.length) {
                    for (; i && i._paused;)i = i._next;
                    i || a.sleep()
                }
            }
        }, a.addEventListener("tick", C._updateRoot);
        var $ = function (t, e, i) {
            var n, r, s = t._gsTweenID;
            if (B[s || (t._gsTweenID = s = "t" + q++)] || (B[s] = {
                    target: t,
                    tweens: []
                }), e && (n = B[s].tweens, n[r = n.length] = e, i))for (; --r > -1;)n[r] === e && n.splice(r, 1);
            return B[s].tweens
        }, K = function (t, e, i, n) {
            var r, s, o = t.vars.onOverwrite;
            return o && (r = o(t, e, i, n)), o = M.onOverwrite, o && (s = o(t, e, i, n)), r !== !1 && s !== !1
        }, J = function (t, e, i, n, r) {
            var s, o, a, l;
            if (1 === n || n >= 4) {
                for (l = r.length, s = 0; l > s; s++)if ((a = r[s]) !== e)a._gc || a._kill(null, t, e) && (o = !0); else if (5 === n)break;
                return o
            }
            var u, h = e._startTime + c, f = [], p = 0, d = 0 === e._duration;
            for (s = r.length; --s > -1;)(a = r[s]) === e || a._gc || a._paused || (a._timeline !== e._timeline ? (u = u || tt(e, 0, d), 0 === tt(a, u, d) && (f[p++] = a)) : a._startTime <= h && a._startTime + a.totalDuration() / a._timeScale > h && ((d || !a._initted) && h - a._startTime <= 2e-10 || (f[p++] = a)));
            for (s = p; --s > -1;)if (a = f[s], 2 === n && a._kill(i, t, e) && (o = !0), 2 !== n || !a._firstPT && a._initted) {
                if (2 !== n && !K(a, e))continue;
                a._enabled(!1, !1) && (o = !0)
            }
            return o
        }, tt = function (t, e, i) {
            for (var n = t._timeline, r = n._timeScale, s = t._startTime; n._timeline;) {
                if (s += n._startTime, r *= n._timeScale, n._paused)return -100;
                n = n._timeline
            }
            return s /= r, s > e ? s - e : i && s === e || !t._initted && 2 * c > s - e ? c : (s += t.totalDuration() / t._timeScale / r) > e + c ? 0 : s - e - c
        };
        o._init = function () {
            var t, e, i, n, r, s = this.vars, o = this._overwrittenProps, a = this._duration, l = !!s.immediateRender, u = s.ease;
            if (s.startAt) {
                this._startAt && (this._startAt.render(-1, !0), this._startAt.kill()), r = {};
                for (n in s.startAt)r[n] = s.startAt[n];
                if (r.overwrite = !1, r.immediateRender = !0, r.lazy = l && s.lazy !== !1, r.startAt = r.delay = null, this._startAt = M.to(this.target, 0, r), l)if (this._time > 0)this._startAt = null; else if (0 !== a)return
            } else if (s.runBackwards && 0 !== a)if (this._startAt)this._startAt.render(-1, !0), this._startAt.kill(), this._startAt = null; else {
                0 !== this._time && (l = !1), i = {};
                for (n in s)U[n] && "autoCSS" !== n || (i[n] = s[n]);
                if (i.overwrite = 0, i.data = "isFromStart", i.lazy = l && s.lazy !== !1, i.immediateRender = l, this._startAt = M.to(this.target, 0, i), l) {
                    if (0 === this._time)return
                } else this._startAt._init(), this._startAt._enabled(!1), this.vars.immediateRender && (this._startAt = null)
            }
            if (this._ease = u = u ? u instanceof x ? u : "function" == typeof u ? new x(u, s.easeParams) : w[u] || M.defaultEase : M.defaultEase, s.easeParams instanceof Array && u.config && (this._ease = u.config.apply(u, s.easeParams)), this._easeType = this._ease._type, this._easePower = this._ease._power, this._firstPT = null, this._targets)for (t = this._targets.length; --t > -1;)this._initProps(this._targets[t], this._propLookup[t] = {}, this._siblings[t], o ? o[t] : null) && (e = !0); else e = this._initProps(this.target, this._propLookup, this._siblings, o);
            if (e && M._onPluginEvent("_onInitAllProps", this), o && (this._firstPT || "function" != typeof this.target && this._enabled(!1, !1)), s.runBackwards)for (i = this._firstPT; i;)i.s += i.c, i.c = -i.c, i = i._next;
            this._onUpdate = s.onUpdate, this._initted = !0
        }, o._initProps = function (e, i, n, r) {
            var s, o, a, l, u, h;
            if (null == e)return !1;
            I[e._gsTweenID] && Z(), this.vars.css || e.style && e !== t && e.nodeType && V.css && this.vars.autoCSS !== !1 && N(this.vars, e);
            for (s in this.vars)if (h = this.vars[s], U[s])h && (h instanceof Array || h.push && d(h)) && -1 !== h.join("").indexOf("{self}") && (this.vars[s] = h = this._swapSelfInParams(h, this)); else if (V[s] && (l = new V[s])._onInitTween(e, this.vars[s], this)) {
                for (this._firstPT = u = {
                    _next: this._firstPT,
                    t: l,
                    p: "setRatio",
                    s: 0,
                    c: 1,
                    f: 1,
                    n: s,
                    pg: 1,
                    pr: l._priority
                }, o = l._overwriteProps.length; --o > -1;)i[l._overwriteProps[o]] = this._firstPT;
                (l._priority || l._onInitAllProps) && (a = !0), (l._onDisable || l._onEnable) && (this._notifyPluginsOfEnabled = !0), u._next && (u._next._prev = u)
            } else i[s] = D.call(this, e, s, "get", h, s, 0, null, this.vars.stringFilter);
            return r && this._kill(r, e) ? this._initProps(e, i, n, r) : this._overwrite > 1 && this._firstPT && n.length > 1 && J(e, this, i, this._overwrite, n) ? (this._kill(i, e), this._initProps(e, i, n, r)) : (this._firstPT && (this.vars.lazy !== !1 && this._duration || this.vars.lazy && !this._duration) && (I[e._gsTweenID] = !0), a)
        }, o.render = function (t, e, i) {
            var n, r, s, o, a = this._time, l = this._duration, u = this._rawPrevTime;
            if (t >= l - 1e-7)this._totalTime = this._time = l, this.ratio = this._ease._calcEnd ? this._ease.getRatio(1) : 1, this._reversed || (n = !0, r = "onComplete", i = i || this._timeline.autoRemoveChildren), 0 === l && (this._initted || !this.vars.lazy || i) && (this._startTime === this._timeline._duration && (t = 0), (0 > u || 0 >= t && t >= -1e-7 || u === c && "isPause" !== this.data) && u !== t && (i = !0, u > c && (r = "onReverseComplete")), this._rawPrevTime = o = !e || t || u === t ? t : c); else if (1e-7 > t)this._totalTime = this._time = 0, this.ratio = this._ease._calcEnd ? this._ease.getRatio(0) : 0, (0 !== a || 0 === l && u > 0) && (r = "onReverseComplete", n = this._reversed), 0 > t && (this._active = !1, 0 === l && (this._initted || !this.vars.lazy || i) && (u >= 0 && (u !== c || "isPause" !== this.data) && (i = !0), this._rawPrevTime = o = !e || t || u === t ? t : c)), this._initted || (i = !0); else if (this._totalTime = this._time = t, this._easeType) {
                var h = t / l, f = this._easeType, p = this._easePower;
                (1 === f || 3 === f && h >= .5) && (h = 1 - h), 3 === f && (h *= 2), 1 === p ? h *= h : 2 === p ? h *= h * h : 3 === p ? h *= h * h * h : 4 === p && (h *= h * h * h * h), 1 === f ? this.ratio = 1 - h : 2 === f ? this.ratio = h : .5 > t / l ? this.ratio = h / 2 : this.ratio = 1 - h / 2
            } else this.ratio = this._ease.getRatio(t / l);
            if (this._time !== a || i) {
                if (!this._initted) {
                    if (this._init(), !this._initted || this._gc)return;
                    if (!i && this._firstPT && (this.vars.lazy !== !1 && this._duration || this.vars.lazy && !this._duration))return this._time = this._totalTime = a, this._rawPrevTime = u, F.push(this), void(this._lazy = [t, e]);
                    this._time && !n ? this.ratio = this._ease.getRatio(this._time / l) : n && this._ease._calcEnd && (this.ratio = this._ease.getRatio(0 === this._time ? 0 : 1))
                }
                for (this._lazy !== !1 && (this._lazy = !1), this._active || !this._paused && this._time !== a && t >= 0 && (this._active = !0), 0 === a && (this._startAt && (t >= 0 ? this._startAt.render(t, e, i) : r || (r = "_dummyGS")), this.vars.onStart && (0 !== this._time || 0 === l) && (e || this._callback("onStart"))), s = this._firstPT; s;)s.f ? s.t[s.p](s.c * this.ratio + s.s) : s.t[s.p] = s.c * this.ratio + s.s, s = s._next;
                this._onUpdate && (0 > t && this._startAt && t !== -1e-4 && this._startAt.render(t, e, i), e || (this._time !== a || n || i) && this._callback("onUpdate")), r && (!this._gc || i) && (0 > t && this._startAt && !this._onUpdate && t !== -1e-4 && this._startAt.render(t, e, i), n && (this._timeline.autoRemoveChildren && this._enabled(!1, !1), this._active = !1), !e && this.vars[r] && this._callback(r), 0 === l && this._rawPrevTime === c && o !== c && (this._rawPrevTime = 0))
            }
        }, o._kill = function (t, e, i) {
            if ("all" === t && (t = null), null == t && (null == e || e === this.target))return this._lazy = !1, this._enabled(!1, !1);
            e = "string" != typeof e ? e || this._targets || this.target : M.selector(e) || e;
            var n, r, s, o, a, l, u, h, c, f = i && this._time && i._startTime === this._startTime && this._timeline === i._timeline;
            if ((d(e) || E(e)) && "number" != typeof e[0])for (n = e.length; --n > -1;)this._kill(t, e[n], i) && (l = !0); else {
                if (this._targets) {
                    for (n = this._targets.length; --n > -1;)if (e === this._targets[n]) {
                        a = this._propLookup[n] || {}, this._overwrittenProps = this._overwrittenProps || [], r = this._overwrittenProps[n] = t ? this._overwrittenProps[n] || {} : "all";
                        break
                    }
                } else {
                    if (e !== this.target)return !1;
                    a = this._propLookup, r = this._overwrittenProps = t ? this._overwrittenProps || {} : "all"
                }
                if (a) {
                    if (u = t || a, h = t !== r && "all" !== r && t !== a && ("object" != typeof t || !t._tempKill), i && (M.onOverwrite || this.vars.onOverwrite)) {
                        for (s in u)a[s] && (c || (c = []), c.push(s));
                        if ((c || !t) && !K(this, i, e, c))return !1
                    }
                    for (s in u)(o = a[s]) && (f && (o.f ? o.t[o.p](o.s) : o.t[o.p] = o.s, l = !0), o.pg && o.t._kill(u) && (l = !0), o.pg && 0 !== o.t._overwriteProps.length || (o._prev ? o._prev._next = o._next : o === this._firstPT && (this._firstPT = o._next),
                    o._next && (o._next._prev = o._prev), o._next = o._prev = null), delete a[s]), h && (r[s] = 1);
                    !this._firstPT && this._initted && this._enabled(!1, !1)
                }
            }
            return l
        }, o.invalidate = function () {
            return this._notifyPluginsOfEnabled && M._onPluginEvent("_onDisable", this), this._firstPT = this._overwrittenProps = this._startAt = this._onUpdate = null, this._notifyPluginsOfEnabled = this._active = this._lazy = !1, this._propLookup = this._targets ? {} : [], C.prototype.invalidate.call(this), this.vars.immediateRender && (this._time = -c, this.render(Math.min(0, -this._delay))), this
        }, o._enabled = function (t, e) {
            if (l || a.wake(), t && this._gc) {
                var i, n = this._targets;
                if (n)for (i = n.length; --i > -1;)this._siblings[i] = $(n[i], this, !0); else this._siblings = $(this.target, this, !0)
            }
            return C.prototype._enabled.call(this, t, e), !(!this._notifyPluginsOfEnabled || !this._firstPT) && M._onPluginEvent(t ? "_onEnable" : "_onDisable", this)
        }, M.to = function (t, e, i) {
            return new M(t, e, i)
        }, M.from = function (t, e, i) {
            return i.runBackwards = !0, i.immediateRender = 0 != i.immediateRender, new M(t, e, i)
        }, M.fromTo = function (t, e, i, n) {
            return n.startAt = i, n.immediateRender = 0 != n.immediateRender && 0 != i.immediateRender, new M(t, e, n)
        }, M.delayedCall = function (t, e, i, n, r) {
            return new M(e, 0, {
                delay: t,
                onComplete: e,
                onCompleteParams: i,
                callbackScope: n,
                onReverseComplete: e,
                onReverseCompleteParams: i,
                immediateRender: !1,
                lazy: !1,
                useFrames: r,
                overwrite: 0
            })
        }, M.set = function (t, e) {
            return new M(t, 0, e)
        }, M.getTweensOf = function (t, e) {
            if (null == t)return [];
            t = "string" != typeof t ? t : M.selector(t) || t;
            var i, n, r, s;
            if ((d(t) || E(t)) && "number" != typeof t[0]) {
                for (i = t.length, n = []; --i > -1;)n = n.concat(M.getTweensOf(t[i], e));
                for (i = n.length; --i > -1;)for (s = n[i], r = i; --r > -1;)s === n[r] && n.splice(i, 1)
            } else for (n = $(t).concat(), i = n.length; --i > -1;)(n[i]._gc || e && !n[i].isActive()) && n.splice(i, 1);
            return n
        }, M.killTweensOf = M.killDelayedCallsTo = function (t, e, i) {
            "object" == typeof e && (i = e, e = !1);
            for (var n = M.getTweensOf(t, e), r = n.length; --r > -1;)n[r]._kill(i, t)
        };
        var et = _("plugins.TweenPlugin", function (t, e) {
            this._overwriteProps = (t || "").split(","), this._propName = this._overwriteProps[0], this._priority = e || 0, this._super = et.prototype
        }, !0);
        if (o = et.prototype, et.version = "1.18.0", et.API = 2, o._firstPT = null, o._addTween = D, o.setRatio = X, o._kill = function (t) {
                var e, i = this._overwriteProps, n = this._firstPT;
                if (null != t[this._propName])this._overwriteProps = []; else for (e = i.length; --e > -1;)null != t[i[e]] && i.splice(e, 1);
                for (; n;)null != t[n.n] && (n._next && (n._next._prev = n._prev), n._prev ? (n._prev._next = n._next, n._prev = null) : this._firstPT === n && (this._firstPT = n._next)), n = n._next;
                return !1
            }, o._roundProps = function (t, e) {
                for (var i = this._firstPT; i;)(t[this._propName] || null != i.n && t[i.n.split(this._propName + "_").join("")]) && (i.r = e), i = i._next
            }, M._onPluginEvent = function (t, e) {
                var i, n, r, s, o, a = e._firstPT;
                if ("_onInitAllProps" === t) {
                    for (; a;) {
                        for (o = a._next, n = r; n && n.pr > a.pr;)n = n._next;
                        (a._prev = n ? n._prev : s) ? a._prev._next = a : r = a, (a._next = n) ? n._prev = a : s = a, a = o
                    }
                    a = e._firstPT = r
                }
                for (; a;)a.pg && "function" == typeof a.t[t] && a.t[t]() && (i = !0), a = a._next;
                return i
            }, et.activate = function (t) {
                for (var e = t.length; --e > -1;)t[e].API === et.API && (V[(new t[e])._propName] = t[e]);
                return !0
            }, g.plugin = function (t) {
                if (!(t && t.propName && t.init && t.API))throw"";
                var e, i = t.propName, n = t.priority || 0, r = t.overwriteProps, s = {
                    init: "_onInitTween",
                    set: "setRatio",
                    kill: "_kill",
                    round: "_roundProps",
                    initAll: "_onInitAllProps"
                }, o = _("plugins." + i.charAt(0).toUpperCase() + i.substr(1) + "Plugin", function () {
                    et.call(this, i, n), this._overwriteProps = r || []
                }, t.global === !0), a = o.prototype = new et(i);
                a.constructor = o, o.API = t.API;
                for (e in s)"function" == typeof t[e] && (a[s[e]] = t[e]);
                return o.version = t.version, et.activate([o]), o
            }, r = t._gsQueue) {
            for (s = 0; s < r.length; s++)r[s]();
            for (o in m)m[o].func || t.console.log("" + o)
        }
        l = !1
    }
}("undefined" != typeof module && module.exports && "undefined" != typeof global ? global : this || window, "TweenLite");
var _gsScope = "undefined" != typeof module && module.exports && "undefined" != typeof global ? global : this || window;
(_gsScope._gsQueue || (_gsScope._gsQueue = [])).push(function () {
    "use strict";
    _gsScope._gsDefine("plugins.CSSPlugin", ["plugins.TweenPlugin", "TweenLite"], function (t, e) {
        var i, n, r, s, o = function () {
            t.call(this, "css"), this._overwriteProps.length = 0, this.setRatio = o.prototype.setRatio
        }, a = _gsScope._gsDefine.globals, l = {}, u = o.prototype = new t("css");
        u.constructor = o, o.version = "1.18.5", o.API = 2, o.defaultTransformPerspective = 0, o.defaultSkewType = "compensated", o.defaultSmoothOrigin = !0, u = "px", o.suffixMap = {
            top: u,
            right: u,
            bottom: u,
            left: u,
            width: u,
            height: u,
            fontSize: u,
            padding: u,
            margin: u,
            perspective: u,
            lineHeight: ""
        };
        var h, c, f, p, d, m, R = /(?:\-|\.|\b)(\d|\.|e\-)+/g, g = /(?:\d|\-\d|\.\d|\-\.\d|\+=\d|\-=\d|\+=.\d|\-=\.\d)+/g, _ = /(?:\+=|\-=|\-|\b)[\d\-\.]+[a-zA-Z0-9]*(?:%|\b)/gi, v = /(?![+-]?\d*\.?\d+|[+-]|e[+-]\d+)[^0-9]/g, y = /(?:\d|\-|\+|=|#|\.)*/g, x = /opacity *= *([^)]*)/i, w = /opacity:([^;]*)/i, b = /alpha\(opacity *=.+?\)/i, T = /^(rgb|hsl)/, z = /([A-Z])/g, P = /-([a-z])/gi, k = /(^(?:url\(\"|url\())|(?:(\"\))$|\)$)/gi, O = function (t, e) {
            return e.toUpperCase()
        }, C = /(?:Left|Right|Width)/i, S = /(M11|M12|M21|M22)=[\d\-\.e]+/gi, A = /progid\:DXImageTransform\.Microsoft\.Matrix\(.+?\)/i, M = /,(?=[^\)]*(?:\(|$))/gi, E = /[\s,\(]/i, N = Math.PI / 180, F = 180 / Math.PI, I = {}, L = document, X = function (t) {
            return L.createElementNS ? L.createElementNS("http://www.w3.org/1999/xhtml", t) : L.createElement(t)
        }, j = X("div"), D = X("img"), Y = o._internals = {_specialProps: l}, V = navigator.userAgent, B = function () {
            var t = V.indexOf("Android"), e = X("a");
            return f = -1 !== V.indexOf("Safari") && -1 === V.indexOf("Chrome") && (-1 === t || Number(V.substr(t + 8, 1)) > 3), d = f && Number(V.substr(V.indexOf("Version/") + 8, 1)) < 6, p = -1 !== V.indexOf("Firefox"), (/MSIE ([0-9]{1,}[\.0-9]{0,})/.exec(V) || /Trident\/.*rv:([0-9]{1,}[\.0-9]{0,})/.exec(V)) && (m = parseFloat(RegExp.$1)), !!e && (e.style.cssText = "top:1px;opacity:.55;", /^0.55/.test(e.style.opacity))
        }(), q = function (t) {
            return x.test("string" == typeof t ? t : (t.currentStyle ? t.currentStyle.filter : t.style.filter) || "") ? parseFloat(RegExp.$1) / 100 : 1
        }, U = function (t) {
            window.console && console.log(t)
        }, G = "", H = "", W = function (t, e) {
            e = e || j;
            var i, n, r = e.style;
            if (void 0 !== r[t])return t;
            for (t = t.charAt(0).toUpperCase() + t.substr(1), i = ["O", "Moz", "ms", "Ms", "Webkit"], n = 5; --n > -1 && void 0 === r[i[n] + t];);
            return n >= 0 ? (H = 3 === n ? "ms" : i[n], G = "-" + H.toLowerCase() + "-", H + t) : null
        }, Q = L.defaultView ? L.defaultView.getComputedStyle : function () {
        }, Z = o.getStyle = function (t, e, i, n, r) {
            var s;
            return B || "opacity" !== e ? (!n && t.style[e] ? s = t.style[e] : (i = i || Q(t)) ? s = i[e] || i.getPropertyValue(e) || i.getPropertyValue(e.replace(z, "-$1").toLowerCase()) : t.currentStyle && (s = t.currentStyle[e]), null == r || s && "none" !== s && "auto" !== s && "auto auto" !== s ? s : r) : q(t)
        }, $ = Y.convertToPixels = function (t, i, n, r, s) {
            if ("px" === r || !r)return n;
            if ("auto" === r || !n)return 0;
            var a, l, u, h = C.test(i), c = t, f = j.style, p = 0 > n, d = 1 === n;
            if (p && (n = -n), d && (n *= 100), "%" === r && -1 !== i.indexOf("border"))a = n / 100 * (h ? t.clientWidth : t.clientHeight); else {
                if (f.cssText = "border:0 solid red;position:" + Z(t, "position") + ";line-height:0;", "%" !== r && c.appendChild && "v" !== r.charAt(0) && "rem" !== r)f[h ? "borderLeftWidth" : "borderTopWidth"] = n + r; else {
                    if (c = t.parentNode || L.body, l = c._gsCache, u = e.ticker.frame, l && h && l.time === u)return l.width * n / 100;
                    f[h ? "width" : "height"] = n + r
                }
                c.appendChild(j), a = parseFloat(j[h ? "offsetWidth" : "offsetHeight"]), c.removeChild(j), h && "%" === r && o.cacheWidths !== !1 && (l = c._gsCache = c._gsCache || {}, l.time = u, l.width = a / n * 100), 0 !== a || s || (a = $(t, i, n, r, !0))
            }
            return d && (a /= 100), p ? -a : a
        }, K = Y.calculateOffset = function (t, e, i) {
            if ("absolute" !== Z(t, "position", i))return 0;
            var n = "left" === e ? "Left" : "Top", r = Z(t, "margin" + n, i);
            return t["offset" + n] - ($(t, e, parseFloat(r), r.replace(y, "")) || 0)
        }, J = function (t, e) {
            var i, n, r, s = {};
            if (e = e || Q(t, null))if (i = e.length)for (; --i > -1;)r = e[i], (-1 === r.indexOf("-transform") || Pt === r) && (s[r.replace(P, O)] = e.getPropertyValue(r)); else for (i in e)(-1 === i.indexOf("Transform") || zt === i) && (s[i] = e[i]); else if (e = t.currentStyle || t.style)for (i in e)"string" == typeof i && void 0 === s[i] && (s[i.replace(P, O)] = e[i]);
            return B || (s.opacity = q(t)), n = jt(t, e, !1), s.rotation = n.rotation, s.skewX = n.skewX, s.scaleX = n.scaleX, s.scaleY = n.scaleY, s.x = n.x, s.y = n.y, Ot && (s.z = n.z, s.rotationX = n.rotationX, s.rotationY = n.rotationY, s.scaleZ = n.scaleZ), s.filters && delete s.filters, s
        }, tt = function (t, e, i, n, r) {
            var s, o, a, l = {}, u = t.style;
            for (o in i)"cssText" !== o && "length" !== o && isNaN(o) && (e[o] !== (s = i[o]) || r && r[o]) && -1 === o.indexOf("Origin") && ("number" == typeof s || "string" == typeof s) && (l[o] = "auto" !== s || "left" !== o && "top" !== o ? "" !== s && "auto" !== s && "none" !== s || "string" != typeof e[o] || "" === e[o].replace(v, "") ? s : 0 : K(t, o), void 0 !== u[o] && (a = new mt(u, o, u[o], a)));
            if (n)for (o in n)"className" !== o && (l[o] = n[o]);
            return {difs: l, firstMPT: a}
        }, et = {
            width: ["Left", "Right"],
            height: ["Top", "Bottom"]
        }, it = ["marginLeft", "marginRight", "marginTop", "marginBottom"], nt = function (t, e, i) {
            if ("svg" === (t.nodeName + "").toLowerCase())return (i || Q(t))[e] || 0;
            if (t.getBBox && It(t))return t.getBBox()[e] || 0;
            var n = parseFloat("width" === e ? t.offsetWidth : t.offsetHeight), r = et[e], s = r.length;
            for (i = i || Q(t, null); --s > -1;)n -= parseFloat(Z(t, "padding" + r[s], i, !0)) || 0, n -= parseFloat(Z(t, "border" + r[s] + "Width", i, !0)) || 0;
            return n
        }, rt = function (t, e) {
            if ("contain" === t || "auto" === t || "auto auto" === t)return t + " ";
            (null == t || "" === t) && (t = "0 0");
            var i, n = t.split(" "), r = -1 !== t.indexOf("left") ? "0%" : -1 !== t.indexOf("right") ? "100%" : n[0], s = -1 !== t.indexOf("top") ? "0%" : -1 !== t.indexOf("bottom") ? "100%" : n[1];
            if (n.length > 3 && !e) {
                for (n = t.split(", ").join(",").split(","), t = [], i = 0; i < n.length; i++)t.push(rt(n[i]));
                return t.join(",")
            }
            return null == s ? s = "center" === r ? "50%" : "0" : "center" === s && (s = "50%"), ("center" === r || isNaN(parseFloat(r)) && -1 === (r + "").indexOf("=")) && (r = "50%"), t = r + " " + s + (n.length > 2 ? " " + n[2] : ""), e && (e.oxp = -1 !== r.indexOf("%"), e.oyp = -1 !== s.indexOf("%"), e.oxr = "=" === r.charAt(1), e.oyr = "=" === s.charAt(1), e.ox = parseFloat(r.replace(v, "")), e.oy = parseFloat(s.replace(v, "")), e.v = t), e || t
        }, st = function (t, e) {
            return "string" == typeof t && "=" === t.charAt(1) ? parseInt(t.charAt(0) + "1", 10) * parseFloat(t.substr(2)) : parseFloat(t) - parseFloat(e) || 0
        }, ot = function (t, e) {
            return null == t ? e : "string" == typeof t && "=" === t.charAt(1) ? parseInt(t.charAt(0) + "1", 10) * parseFloat(t.substr(2)) + e : parseFloat(t) || 0
        }, at = function (t, e, i, n) {
            var r, s, o, a, l, u = 1e-6;
            return null == t ? a = e : "number" == typeof t ? a = t : (r = 360, s = t.split("_"), l = "=" === t.charAt(1), o = (l ? parseInt(t.charAt(0) + "1", 10) * parseFloat(s[0].substr(2)) : parseFloat(s[0])) * (-1 === t.indexOf("rad") ? 1 : F) - (l ? 0 : e), s.length && (n && (n[i] = e + o), -1 !== t.indexOf("short") && (o %= r, o !== o % (r / 2) && (o = 0 > o ? o + r : o - r)), -1 !== t.indexOf("_cw") && 0 > o ? o = (o + 9999999999 * r) % r - (o / r | 0) * r : -1 !== t.indexOf("ccw") && o > 0 && (o = (o - 9999999999 * r) % r - (o / r | 0) * r)), a = e + o), u > a && a > -u && (a = 0), a
        }, lt = {
            aqua: [0, 255, 255],
            lime: [0, 255, 0],
            silver: [192, 192, 192],
            black: [0, 0, 0],
            maroon: [128, 0, 0],
            teal: [0, 128, 128],
            blue: [0, 0, 255],
            navy: [0, 0, 128],
            white: [255, 255, 255],
            fuchsia: [255, 0, 255],
            olive: [128, 128, 0],
            yellow: [255, 255, 0],
            orange: [255, 165, 0],
            gray: [128, 128, 128],
            purple: [128, 0, 128],
            green: [0, 128, 0],
            red: [255, 0, 0],
            pink: [255, 192, 203],
            cyan: [0, 255, 255],
            transparent: [255, 255, 255, 0]
        }, ut = function (t, e, i) {
            return t = 0 > t ? t + 1 : t > 1 ? t - 1 : t, 255 * (1 > 6 * t ? e + (i - e) * t * 6 : .5 > t ? i : 2 > 3 * t ? e + (i - e) * (2 / 3 - t) * 6 : e) + .5 | 0
        }, ht = o.parseColor = function (t, e) {
            var i, n, r, s, o, a, l, u, h, c, f;
            if (t)if ("number" == typeof t)i = [t >> 16, t >> 8 & 255, 255 & t]; else {
                if ("," === t.charAt(t.length - 1) && (t = t.substr(0, t.length - 1)), lt[t])i = lt[t]; else if ("#" === t.charAt(0))4 === t.length && (n = t.charAt(1), r = t.charAt(2), s = t.charAt(3), t = "#" + n + n + r + r + s + s), t = parseInt(t.substr(1), 16), i = [t >> 16, t >> 8 & 255, 255 & t]; else if ("hsl" === t.substr(0, 3))if (i = f = t.match(R), e) {
                    if (-1 !== t.indexOf("="))return t.match(g)
                } else o = Number(i[0]) % 360 / 360, a = Number(i[1]) / 100, l = Number(i[2]) / 100, r = .5 >= l ? l * (a + 1) : l + a - l * a, n = 2 * l - r, i.length > 3 && (i[3] = Number(t[3])), i[0] = ut(o + 1 / 3, n, r), i[1] = ut(o, n, r), i[2] = ut(o - 1 / 3, n, r); else i = t.match(R) || lt.transparent;
                i[0] = Number(i[0]), i[1] = Number(i[1]), i[2] = Number(i[2]), i.length > 3 && (i[3] = Number(i[3]))
            } else i = lt.black;
            return e && !f && (n = i[0] / 255, r = i[1] / 255, s = i[2] / 255, u = Math.max(n, r, s), h = Math.min(n, r, s), l = (u + h) / 2, u === h ? o = a = 0 : (c = u - h, a = l > .5 ? c / (2 - u - h) : c / (u + h), o = u === n ? (r - s) / c + (s > r ? 6 : 0) : u === r ? (s - n) / c + 2 : (n - r) / c + 4, o *= 60), i[0] = o + .5 | 0, i[1] = 100 * a + .5 | 0, i[2] = 100 * l + .5 | 0), i
        }, ct = function (t, e) {
            var i, n, r, s = t.match(ft) || [], o = 0, a = s.length ? "" : t;
            for (i = 0; i < s.length; i++)n = s[i], r = t.substr(o, t.indexOf(n, o) - o), o += r.length + n.length, n = ht(n, e), 3 === n.length && n.push(1), a += r + (e ? "hsla(" + n[0] + "," + n[1] + "%," + n[2] + "%," + n[3] : "rgba(" + n.join(",")) + ")";
            return a + t.substr(o)
        }, ft = "(?:\\b(?:(?:rgb|rgba|hsl|hsla)\\(.+?\\))|\\B#(?:[0-9a-f]{3}){1,2}\\b";
        for (u in lt)ft += "|" + u + "\\b";
        ft = new RegExp(ft + ")", "gi"), o.colorStringFilter = function (t) {
            var e, i = t[0] + t[1];
            ft.test(i) && (e = -1 !== i.indexOf("hsl(") || -1 !== i.indexOf("hsla("), t[0] = ct(t[0], e), t[1] = ct(t[1], e)), ft.lastIndex = 0
        }, e.defaultStringFilter || (e.defaultStringFilter = o.colorStringFilter);
        var pt = function (t, e, i, n) {
            if (null == t)return function (t) {
                return t
            };
            var r, s = e ? (t.match(ft) || [""])[0] : "", o = t.split(s).join("").match(_) || [], a = t.substr(0, t.indexOf(o[0])), l = ")" === t.charAt(t.length - 1) ? ")" : "", u = -1 !== t.indexOf(" ") ? " " : ",", h = o.length, c = h > 0 ? o[0].replace(R, "") : "";
            return h ? r = e ? function (t) {
                var e, f, p, d;
                if ("number" == typeof t)t += c; else if (n && M.test(t)) {
                    for (d = t.replace(M, "|").split("|"), p = 0; p < d.length; p++)d[p] = r(d[p]);
                    return d.join(",")
                }
                if (e = (t.match(ft) || [s])[0], f = t.split(e).join("").match(_) || [], p = f.length, h > p--)for (; ++p < h;)f[p] = i ? f[(p - 1) / 2 | 0] : o[p];
                return a + f.join(u) + u + e + l + (-1 !== t.indexOf("inset") ? " inset" : "")
            } : function (t) {
                var e, s, f;
                if ("number" == typeof t)t += c; else if (n && M.test(t)) {
                    for (s = t.replace(M, "|").split("|"), f = 0; f < s.length; f++)s[f] = r(s[f]);
                    return s.join(",")
                }
                if (e = t.match(_) || [], f = e.length, h > f--)for (; ++f < h;)e[f] = i ? e[(f - 1) / 2 | 0] : o[f];
                return a + e.join(u) + l
            } : function (t) {
                return t
            }
        }, dt = function (t) {
            return t = t.split(","), function (e, i, n, r, s, o, a) {
                var l, u = (i + "").split(" ");
                for (a = {}, l = 0; 4 > l; l++)a[t[l]] = u[l] = u[l] || u[(l - 1) / 2 >> 0];
                return r.parse(e, a, s, o)
            }
        }, mt = (Y._setPluginRatio = function (t) {
            this.plugin.setRatio(t);
            for (var e, i, n, r, s, o = this.data, a = o.proxy, l = o.firstMPT, u = 1e-6; l;)e = a[l.v], l.r ? e = Math.round(e) : u > e && e > -u && (e = 0), l.t[l.p] = e, l = l._next;
            if (o.autoRotate && (o.autoRotate.rotation = a.rotation), 1 === t || 0 === t)for (l = o.firstMPT, s = 1 === t ? "e" : "b"; l;) {
                if (i = l.t, i.type) {
                    if (1 === i.type) {
                        for (r = i.xs0 + i.s + i.xs1, n = 1; n < i.l; n++)r += i["xn" + n] + i["xs" + (n + 1)];
                        i[s] = r
                    }
                } else i[s] = i.s + i.xs0;
                l = l._next
            }
        }, function (t, e, i, n, r) {
            this.t = t, this.p = e, this.v = i, this.r = r, n && (n._prev = this, this._next = n)
        }), Rt = (Y._parseToProxy = function (t, e, i, n, r, s) {
            var o, a, l, u, h, c = n, f = {}, p = {}, d = i._transform, m = I;
            for (i._transform = null, I = e, n = h = i.parse(t, e, n, r), I = m, s && (i._transform = d, c && (c._prev = null, c._prev && (c._prev._next = null))); n && n !== c;) {
                if (n.type <= 1 && (a = n.p, p[a] = n.s + n.c, f[a] = n.s, s || (u = new mt(n, "s", a, u, n.r), n.c = 0), 1 === n.type))for (o = n.l; --o > 0;)l = "xn" + o, a = n.p + "_" + l, p[a] = n.data[l], f[a] = n[l], s || (u = new mt(n, l, a, u, n.rxp[l]));
                n = n._next
            }
            return {proxy: f, end: p, firstMPT: u, pt: h}
        }, Y.CSSPropTween = function (t, e, n, r, o, a, l, u, h, c, f) {
            this.t = t, this.p = e, this.s = n, this.c = r, this.n = l || e, t instanceof Rt || s.push(this.n), this.r = u, this.type = a || 0, h && (this.pr = h, i = !0), this.b = void 0 === c ? n : c, this.e = void 0 === f ? n + r : f, o && (this._next = o, o._prev = this)
        }), gt = function (t, e, i, n, r, s) {
            var o = new Rt(t, e, i, n - i, r, (-1), s);
            return o.b = i, o.e = o.xs0 = n, o
        }, _t = o.parseComplex = function (t, e, i, n, r, s, a, l, u, c) {
            i = i || s || "", a = new Rt(t, e, 0, 0, a, c ? 2 : 1, null, (!1), l, i, n), n += "", r && ft.test(n + i) && (n = [i, n], o.colorStringFilter(n), i = n[0], n = n[1]);
            var f, p, d, m, _, v, y, x, w, b, T, z, P, k = i.split(", ").join(",").split(" "), O = n.split(", ").join(",").split(" "), C = k.length, S = h !== !1;
            for ((-1 !== n.indexOf(",") || -1 !== i.indexOf(",")) && (k = k.join(" ").replace(M, ", ").split(" "), O = O.join(" ").replace(M, ", ").split(" "), C = k.length), C !== O.length && (k = (s || "").split(" "), C = k.length), a.plugin = u, a.setRatio = c, ft.lastIndex = 0, f = 0; C > f; f++)if (m = k[f], _ = O[f], x = parseFloat(m), x || 0 === x)a.appendXtra("", x, st(_, x), _.replace(g, ""), S && -1 !== _.indexOf("px"), !0); else if (r && ft.test(m))z = _.indexOf(")") + 1, z = ")" + (z ? _.substr(z) : ""), P = -1 !== _.indexOf("hsl") && B, m = ht(m, P), _ = ht(_, P), w = m.length + _.length > 6, w && !B && 0 === _[3] ? (a["xs" + a.l] += a.l ? " transparent" : "transparent", a.e = a.e.split(O[f]).join("transparent")) : (B || (w = !1), P ? a.appendXtra(w ? "hsla(" : "hsl(", m[0], st(_[0], m[0]), ",", !1, !0).appendXtra("", m[1], st(_[1], m[1]), "%,", !1).appendXtra("", m[2], st(_[2], m[2]), w ? "%," : "%" + z, !1) : a.appendXtra(w ? "rgba(" : "rgb(", m[0], _[0] - m[0], ",", !0, !0).appendXtra("", m[1], _[1] - m[1], ",", !0).appendXtra("", m[2], _[2] - m[2], w ? "," : z, !0), w && (m = m.length < 4 ? 1 : m[3], a.appendXtra("", m, (_.length < 4 ? 1 : _[3]) - m, z, !1))), ft.lastIndex = 0; else if (v = m.match(R)) {
                if (y = _.match(g), !y || y.length !== v.length)return a;
                for (d = 0, p = 0; p < v.length; p++)T = v[p], b = m.indexOf(T, d), a.appendXtra(m.substr(d, b - d), Number(T), st(y[p], T), "", S && "px" === m.substr(b + T.length, 2), 0 === p), d = b + T.length;
                a["xs" + a.l] += m.substr(d)
            } else a["xs" + a.l] += a.l || a["xs" + a.l] ? " " + _ : _;
            if (-1 !== n.indexOf("=") && a.data) {
                for (z = a.xs0 + a.data.s, f = 1; f < a.l; f++)z += a["xs" + f] + a.data["xn" + f];
                a.e = z + a["xs" + f]
            }
            return a.l || (a.type = -1, a.xs0 = a.e), a.xfirst || a
        }, vt = 9;
        for (u = Rt.prototype, u.l = u.pr = 0; --vt > 0;)u["xn" + vt] = 0, u["xs" + vt] = "";
        u.xs0 = "", u._next = u._prev = u.xfirst = u.data = u.plugin = u.setRatio = u.rxp = null, u.appendXtra = function (t, e, i, n, r, s) {
            var o = this, a = o.l;
            return o["xs" + a] += s && (a || o["xs" + a]) ? " " + t : t || "", i || 0 === a || o.plugin ? (o.l++, o.type = o.setRatio ? 2 : 1, o["xs" + o.l] = n || "", a > 0 ? (o.data["xn" + a] = e + i, o.rxp["xn" + a] = r, o["xn" + a] = e, o.plugin || (o.xfirst = new Rt(o, "xn" + a, e, i, o.xfirst || o, 0, o.n, r, o.pr), o.xfirst.xs0 = 0), o) : (o.data = {s: e + i}, o.rxp = {}, o.s = e, o.c = i, o.r = r, o)) : (o["xs" + a] += e + (n || ""), o)
        };
        var yt = function (t, e) {
            e = e || {}, this.p = e.prefix ? W(t) || t : t, l[t] = l[this.p] = this, this.format = e.formatter || pt(e.defaultValue, e.color, e.collapsible, e.multi), e.parser && (this.parse = e.parser), this.clrs = e.color, this.multi = e.multi, this.keyword = e.keyword, this.dflt = e.defaultValue, this.pr = e.priority || 0
        }, xt = Y._registerComplexSpecialProp = function (t, e, i) {
            "object" != typeof e && (e = {parser: i});
            var n, r, s = t.split(","), o = e.defaultValue;
            for (i = i || [o], n = 0; n < s.length; n++)e.prefix = 0 === n && e.prefix, e.defaultValue = i[n] || o, r = new yt(s[n], e)
        }, wt = function (t) {
            if (!l[t]) {
                var e = t.charAt(0).toUpperCase() + t.substr(1) + "Plugin";
                xt(t, {
                    parser: function (t, i, n, r, s, o, u) {
                        var h = a.com.greensock.plugins[e];
                        return h ? (h._cssRegister(), l[n].parse(t, i, n, r, s, o, u)) : (U("Error: " + e + " js file not loaded."), s)
                    }
                })
            }
        };
        u = yt.prototype, u.parseComplex = function (t, e, i, n, r, s) {
            var o, a, l, u, h, c, f = this.keyword;
            if (this.multi && (M.test(i) || M.test(e) ? (a = e.replace(M, "|").split("|"), l = i.replace(M, "|").split("|")) : f && (a = [e], l = [i])), l) {
                for (u = l.length > a.length ? l.length : a.length, o = 0; u > o; o++)e = a[o] = a[o] || this.dflt, i = l[o] = l[o] || this.dflt, f && (h = e.indexOf(f), c = i.indexOf(f), h !== c && (-1 === c ? a[o] = a[o].split(f).join("") : -1 === h && (a[o] += " " + f)));
                e = a.join(", "), i = l.join(", ")
            }
            return _t(t, this.p, e, i, this.clrs, this.dflt, n, this.pr, r, s)
        }, u.parse = function (t, e, i, n, s, o, a) {
            return this.parseComplex(t.style, this.format(Z(t, this.p, r, !1, this.dflt)), this.format(e), s, o)
        }, o.registerSpecialProp = function (t, e, i) {
            xt(t, {
                parser: function (t, n, r, s, o, a, l) {
                    var u = new Rt(t, r, 0, 0, o, 2, r, (!1), i);
                    return u.plugin = a, u.setRatio = e(t, n, s._tween, r), u
                }, priority: i
            })
        }, o.useSVGTransformAttr = f || p;
        var bt, Tt = "scaleX,scaleY,scaleZ,x,y,z,skewX,skewY,rotation,rotationX,rotationY,perspective,xPercent,yPercent".split(","), zt = W("transform"), Pt = G + "transform", kt = W("transformOrigin"), Ot = null !== W("perspective"), Ct = Y.Transform = function () {
            this.perspective = parseFloat(o.defaultTransformPerspective) || 0, this.force3D = !(o.defaultForce3D === !1 || !Ot) && (o.defaultForce3D || "auto")
        }, St = window.SVGElement, At = function (t, e, i) {
            var n, r = L.createElementNS("http://www.w3.org/2000/svg", t), s = /([a-z])([A-Z])/g;
            for (n in i)r.setAttributeNS(null, n.replace(s, "$1-$2").toLowerCase(), i[n]);
            return e.appendChild(r), r
        }, Mt = L.documentElement, Et = function () {
            var t, e, i, n = m || /Android/i.test(V) && !window.chrome;
            return L.createElementNS && !n && (t = At("svg", Mt), e = At("rect", t, {
                width: 100,
                height: 50,
                x: 100
            }), i = e.getBoundingClientRect().width, e.style[kt] = "50% 50%", e.style[zt] = "scaleX(0.5)", n = i === e.getBoundingClientRect().width && !(p && Ot), Mt.removeChild(t)), n
        }(), Nt = function (t, e, i, n, r, s) {
            var a, l, u, h, c, f, p, d, m, R, g, _, v, y, x = t._gsTransform, w = Xt(t, !0);
            x && (v = x.xOrigin, y = x.yOrigin), (!n || (a = n.split(" ")).length < 2) && (p = t.getBBox(), e = rt(e).split(" "), a = [(-1 !== e[0].indexOf("%") ? parseFloat(e[0]) / 100 * p.width : parseFloat(e[0])) + p.x, (-1 !== e[1].indexOf("%") ? parseFloat(e[1]) / 100 * p.height : parseFloat(e[1])) + p.y]), i.xOrigin = h = parseFloat(a[0]), i.yOrigin = c = parseFloat(a[1]), n && w !== Lt && (f = w[0], p = w[1], d = w[2], m = w[3], R = w[4], g = w[5], _ = f * m - p * d, l = h * (m / _) + c * (-d / _) + (d * g - m * R) / _, u = h * (-p / _) + c * (f / _) - (f * g - p * R) / _, h = i.xOrigin = a[0] = l, c = i.yOrigin = a[1] = u), x && (s && (i.xOffset = x.xOffset, i.yOffset = x.yOffset, x = i), r || r !== !1 && o.defaultSmoothOrigin !== !1 ? (l = h - v, u = c - y, x.xOffset += l * w[0] + u * w[2] - l, x.yOffset += l * w[1] + u * w[3] - u) : x.xOffset = x.yOffset = 0), s || t.setAttribute("data-svg-origin", a.join(" "))
        }, Ft = function (t) {
            try {
                return t.getBBox()
            } catch (t) {
            }
        }, It = function (t) {
            return !!(St && t.getBBox && t.getCTM && Ft(t) && (!t.parentNode || t.parentNode.getBBox && t.parentNode.getCTM))
        }, Lt = [1, 0, 0, 1, 0, 0], Xt = function (t, e) {
            var i, n, r, s, o, a, l = t._gsTransform || new Ct, u = 1e5, h = t.style;
            if (zt ? n = Z(t, Pt, null, !0) : t.currentStyle && (n = t.currentStyle.filter.match(S), n = n && 4 === n.length ? [n[0].substr(4), Number(n[2].substr(4)), Number(n[1].substr(4)), n[3].substr(4), l.x || 0, l.y || 0].join(",") : ""), i = !n || "none" === n || "matrix(1, 0, 0, 1, 0, 0)" === n, i && zt && ((a = "none" === Q(t).display) || !t.parentNode) && (a && (s = h.display, h.display = "block"), t.parentNode || (o = 1, Mt.appendChild(t)), n = Z(t, Pt, null, !0), i = !n || "none" === n || "matrix(1, 0, 0, 1, 0, 0)" === n, s ? h.display = s : a && Bt(h, "display"), o && Mt.removeChild(t)), (l.svg || t.getBBox && It(t)) && (i && -1 !== (h[zt] + "").indexOf("matrix") && (n = h[zt], i = 0), r = t.getAttribute("transform"), i && r && (-1 !== r.indexOf("matrix") ? (n = r, i = 0) : -1 !== r.indexOf("translate") && (n = "matrix(1,0,0,1," + r.match(/(?:\-|\b)[\d\-\.e]+\b/gi).join(",") + ")", i = 0))), i)return Lt;
            for (r = (n || "").match(R) || [], vt = r.length; --vt > -1;)s = Number(r[vt]), r[vt] = (o = s - (s |= 0)) ? (o * u + (0 > o ? -.5 : .5) | 0) / u + s : s;
            return e && r.length > 6 ? [r[0], r[1], r[4], r[5], r[12], r[13]] : r
        }, jt = Y.getTransform = function (t, i, n, r) {
            if (t._gsTransform && n && !r)return t._gsTransform;
            var s, a, l, u, h, c, f = n ? t._gsTransform || new Ct : new Ct, p = f.scaleX < 0, d = 2e-5, m = 1e5, R = Ot ? parseFloat(Z(t, kt, i, !1, "0 0 0").split(" ")[2]) || f.zOrigin || 0 : 0, g = parseFloat(o.defaultTransformPerspective) || 0;
            if (f.svg = !(!t.getBBox || !It(t)), f.svg && (Nt(t, Z(t, kt, i, !1, "50% 50%") + "", f, t.getAttribute("data-svg-origin")), bt = o.useSVGTransformAttr || Et), s = Xt(t), s !== Lt) {
                if (16 === s.length) {
                    var _, v, y, x, w, b = s[0], T = s[1], z = s[2], P = s[3], k = s[4], O = s[5], C = s[6], S = s[7], A = s[8], M = s[9], E = s[10], N = s[12], I = s[13], L = s[14], X = s[11], j = Math.atan2(C, E);
                    f.zOrigin && (L = -f.zOrigin, N = A * L - s[12], I = M * L - s[13], L = E * L + f.zOrigin - s[14]), f.rotationX = j * F, j && (x = Math.cos(-j), w = Math.sin(-j), _ = k * x + A * w, v = O * x + M * w, y = C * x + E * w, A = k * -w + A * x, M = O * -w + M * x, E = C * -w + E * x, X = S * -w + X * x, k = _, O = v, C = y), j = Math.atan2(-z, E), f.rotationY = j * F, j && (x = Math.cos(-j), w = Math.sin(-j), _ = b * x - A * w, v = T * x - M * w, y = z * x - E * w, M = T * w + M * x, E = z * w + E * x, X = P * w + X * x, b = _, T = v, z = y), j = Math.atan2(T, b), f.rotation = j * F, j && (x = Math.cos(-j), w = Math.sin(-j), b = b * x + k * w, v = T * x + O * w, O = T * -w + O * x, C = z * -w + C * x, T = v), f.rotationX && Math.abs(f.rotationX) + Math.abs(f.rotation) > 359.9 && (f.rotationX = f.rotation = 0, f.rotationY = 180 - f.rotationY), f.scaleX = (Math.sqrt(b * b + T * T) * m + .5 | 0) / m, f.scaleY = (Math.sqrt(O * O + M * M) * m + .5 | 0) / m, f.scaleZ = (Math.sqrt(C * C + E * E) * m + .5 | 0) / m, f.rotationX || f.rotationY ? f.skewX = 0 : (f.skewX = k || O ? Math.atan2(k, O) * F + f.rotation : f.skewX || 0, Math.abs(f.skewX) > 90 && Math.abs(f.skewX) < 270 && (p ? (f.scaleX *= -1, f.skewX += f.rotation <= 0 ? 180 : -180, f.rotation += f.rotation <= 0 ? 180 : -180) : (f.scaleY *= -1, f.skewX += f.skewX <= 0 ? 180 : -180))), f.perspective = X ? 1 / (0 > X ? -X : X) : 0, f.x = N, f.y = I, f.z = L, f.svg && (f.x -= f.xOrigin - (f.xOrigin * b - f.yOrigin * k), f.y -= f.yOrigin - (f.yOrigin * T - f.xOrigin * O))
                } else if (!Ot || r || !s.length || f.x !== s[4] || f.y !== s[5] || !f.rotationX && !f.rotationY) {
                    var D = s.length >= 6, Y = D ? s[0] : 1, V = s[1] || 0, B = s[2] || 0, q = D ? s[3] : 1;
                    f.x = s[4] || 0, f.y = s[5] || 0, l = Math.sqrt(Y * Y + V * V), u = Math.sqrt(q * q + B * B), h = Y || V ? Math.atan2(V, Y) * F : f.rotation || 0, c = B || q ? Math.atan2(B, q) * F + h : f.skewX || 0, Math.abs(c) > 90 && Math.abs(c) < 270 && (p ? (l *= -1, c += 0 >= h ? 180 : -180, h += 0 >= h ? 180 : -180) : (u *= -1, c += 0 >= c ? 180 : -180)), f.scaleX = l, f.scaleY = u, f.rotation = h, f.skewX = c, Ot && (f.rotationX = f.rotationY = f.z = 0, f.perspective = g, f.scaleZ = 1), f.svg && (f.x -= f.xOrigin - (f.xOrigin * Y + f.yOrigin * B), f.y -= f.yOrigin - (f.xOrigin * V + f.yOrigin * q))
                }
                f.zOrigin = R;
                for (a in f)f[a] < d && f[a] > -d && (f[a] = 0)
            }
            return n && (t._gsTransform = f, f.svg && (bt && t.style[zt] ? e.delayedCall(.001, function () {
                Bt(t.style, zt)
            }) : !bt && t.getAttribute("transform") && e.delayedCall(.001, function () {
                t.removeAttribute("transform")
            }))), f
        }, Dt = function (t) {
            var e, i, n = this.data, r = -n.rotation * N, s = r + n.skewX * N, o = 1e5, a = (Math.cos(r) * n.scaleX * o | 0) / o, l = (Math.sin(r) * n.scaleX * o | 0) / o, u = (Math.sin(s) * -n.scaleY * o | 0) / o, h = (Math.cos(s) * n.scaleY * o | 0) / o, c = this.t.style, f = this.t.currentStyle;
            if (f) {
                i = l, l = -u, u = -i, e = f.filter, c.filter = "";
                var p, d, R = this.t.offsetWidth, g = this.t.offsetHeight, _ = "absolute" !== f.position, v = "progid:DXImageTransform.Microsoft.Matrix(M11=" + a + ", M12=" + l + ", M21=" + u + ", M22=" + h, w = n.x + R * n.xPercent / 100, b = n.y + g * n.yPercent / 100;
                if (null != n.ox && (p = (n.oxp ? R * n.ox * .01 : n.ox) - R / 2, d = (n.oyp ? g * n.oy * .01 : n.oy) - g / 2, w += p - (p * a + d * l), b += d - (p * u + d * h)), _ ? (p = R / 2, d = g / 2, v += ", Dx=" + (p - (p * a + d * l) + w) + ", Dy=" + (d - (p * u + d * h) + b) + ")") : v += ", sizingMethod='auto expand')", -1 !== e.indexOf("DXImageTransform.Microsoft.Matrix(") ? c.filter = e.replace(A, v) : c.filter = v + " " + e, (0 === t || 1 === t) && 1 === a && 0 === l && 0 === u && 1 === h && (_ && -1 === v.indexOf("Dx=0, Dy=0") || x.test(e) && 100 !== parseFloat(RegExp.$1) || -1 === e.indexOf(e.indexOf("Alpha")) && c.removeAttribute("filter")), !_) {
                    var T, z, P, k = 8 > m ? 1 : -1;
                    for (p = n.ieOffsetX || 0, d = n.ieOffsetY || 0, n.ieOffsetX = Math.round((R - ((0 > a ? -a : a) * R + (0 > l ? -l : l) * g)) / 2 + w), n.ieOffsetY = Math.round((g - ((0 > h ? -h : h) * g + (0 > u ? -u : u) * R)) / 2 + b), vt = 0; 4 > vt; vt++)z = it[vt], T = f[z], i = -1 !== T.indexOf("px") ? parseFloat(T) : $(this.t, z, parseFloat(T), T.replace(y, "")) || 0, P = i !== n[z] ? 2 > vt ? -n.ieOffsetX : -n.ieOffsetY : 2 > vt ? p - n.ieOffsetX : d - n.ieOffsetY, c[z] = (n[z] = Math.round(i - P * (0 === vt || 2 === vt ? 1 : k))) + "px"
                }
            }
        }, Yt = Y.set3DTransformRatio = Y.setTransformRatio = function (t) {
            var e, i, n, r, s, o, a, l, u, h, c, f, d, m, R, g, _, v, y, x, w, b, T, z = this.data, P = this.t.style, k = z.rotation, O = z.rotationX, C = z.rotationY, S = z.scaleX, A = z.scaleY, M = z.scaleZ, E = z.x, F = z.y, I = z.z, L = z.svg, X = z.perspective, j = z.force3D;
            if (((1 === t || 0 === t) && "auto" === j && (this.tween._totalTime === this.tween._totalDuration || !this.tween._totalTime) || !j) && !I && !X && !C && !O && 1 === M || bt && L || !Ot)return void(k || z.skewX || L ? (k *= N, b = z.skewX * N, T = 1e5, e = Math.cos(k) * S, r = Math.sin(k) * S, i = Math.sin(k - b) * -A, s = Math.cos(k - b) * A, b && "simple" === z.skewType && (_ = Math.tan(b), _ = Math.sqrt(1 + _ * _), i *= _, s *= _, z.skewY && (e *= _, r *= _)), L && (E += z.xOrigin - (z.xOrigin * e + z.yOrigin * i) + z.xOffset, F += z.yOrigin - (z.xOrigin * r + z.yOrigin * s) + z.yOffset, bt && (z.xPercent || z.yPercent) && (m = this.t.getBBox(), E += .01 * z.xPercent * m.width, F += .01 * z.yPercent * m.height), m = 1e-6, m > E && E > -m && (E = 0), m > F && F > -m && (F = 0)), y = (e * T | 0) / T + "," + (r * T | 0) / T + "," + (i * T | 0) / T + "," + (s * T | 0) / T + "," + E + "," + F + ")", L && bt ? this.t.setAttribute("transform", "matrix(" + y) : P[zt] = (z.xPercent || z.yPercent ? "translate(" + z.xPercent + "%," + z.yPercent + "%) matrix(" : "matrix(") + y) : P[zt] = (z.xPercent || z.yPercent ? "translate(" + z.xPercent + "%," + z.yPercent + "%) matrix(" : "matrix(") + S + ",0,0," + A + "," + E + "," + F + ")");
            if (p && (m = 1e-4, m > S && S > -m && (S = M = 2e-5), m > A && A > -m && (A = M = 2e-5), !X || z.z || z.rotationX || z.rotationY || (X = 0)), k || z.skewX)k *= N, R = e = Math.cos(k), g = r = Math.sin(k), z.skewX && (k -= z.skewX * N, R = Math.cos(k), g = Math.sin(k), "simple" === z.skewType && (_ = Math.tan(z.skewX * N), _ = Math.sqrt(1 + _ * _), R *= _, g *= _, z.skewY && (e *= _, r *= _))), i = -g, s = R; else {
                if (!(C || O || 1 !== M || X || L))return void(P[zt] = (z.xPercent || z.yPercent ? "translate(" + z.xPercent + "%," + z.yPercent + "%) translate3d(" : "translate3d(") + E + "px," + F + "px," + I + "px)" + (1 !== S || 1 !== A ? " scale(" + S + "," + A + ")" : ""));
                e = s = 1, i = r = 0
            }
            u = 1, n = o = a = l = h = c = 0, f = X ? -1 / X : 0, d = z.zOrigin, m = 1e-6, x = ",", w = "0", k = C * N, k && (R = Math.cos(k), g = Math.sin(k), a = -g, h = f * -g, n = e * g, o = r * g, u = R, f *= R, e *= R, r *= R), k = O * N, k && (R = Math.cos(k), g = Math.sin(k), _ = i * R + n * g, v = s * R + o * g, l = u * g, c = f * g, n = i * -g + n * R, o = s * -g + o * R, u *= R, f *= R, i = _, s = v), 1 !== M && (n *= M, o *= M, u *= M, f *= M), 1 !== A && (i *= A, s *= A, l *= A, c *= A), 1 !== S && (e *= S, r *= S, a *= S, h *= S), (d || L) && (d && (E += n * -d, F += o * -d, I += u * -d + d), L && (E += z.xOrigin - (z.xOrigin * e + z.yOrigin * i) + z.xOffset, F += z.yOrigin - (z.xOrigin * r + z.yOrigin * s) + z.yOffset), m > E && E > -m && (E = w), m > F && F > -m && (F = w), m > I && I > -m && (I = 0)), y = z.xPercent || z.yPercent ? "translate(" + z.xPercent + "%," + z.yPercent + "%) matrix3d(" : "matrix3d(", y += (m > e && e > -m ? w : e) + x + (m > r && r > -m ? w : r) + x + (m > a && a > -m ? w : a), y += x + (m > h && h > -m ? w : h) + x + (m > i && i > -m ? w : i) + x + (m > s && s > -m ? w : s), O || C || 1 !== M ? (y += x + (m > l && l > -m ? w : l) + x + (m > c && c > -m ? w : c) + x + (m > n && n > -m ? w : n), y += x + (m > o && o > -m ? w : o) + x + (m > u && u > -m ? w : u) + x + (m > f && f > -m ? w : f) + x) : y += ",0,0,0,0,1,0,", y += E + x + F + x + I + x + (X ? 1 + -I / X : 1) + ")", P[zt] = y
        };
        u = Ct.prototype, u.x = u.y = u.z = u.skewX = u.skewY = u.rotation = u.rotationX = u.rotationY = u.zOrigin = u.xPercent = u.yPercent = u.xOffset = u.yOffset = 0, u.scaleX = u.scaleY = u.scaleZ = 1, xt("transform,scale,scaleX,scaleY,scaleZ,x,y,z,rotation,rotationX,rotationY,rotationZ,skewX,skewY,shortRotation,shortRotationX,shortRotationY,shortRotationZ,transformOrigin,svgOrigin,transformPerspective,directionalRotation,parseTransform,force3D,skewType,xPercent,yPercent,smoothOrigin", {
            parser: function (t, e, i, n, s, a, l) {
                if (n._lastParsedTransform === l)return s;
                n._lastParsedTransform = l;
                var u, h, c, f, p, d, m, R, g, _ = t._gsTransform, v = t.style, y = 1e-6, x = Tt.length, w = l, b = {}, T = "transformOrigin", z = jt(t, r, !0, l.parseTransform);
                if (n._transform = z, "string" == typeof w.transform && zt)h = j.style, h[zt] = w.transform, h.display = "block", h.position = "absolute", L.body.appendChild(j), u = jt(j, null, !1), z.svg && (m = z.xOrigin, R = z.yOrigin, u.x -= z.xOffset, u.y -= z.yOffset, (w.transformOrigin || w.svgOrigin) && (c = {}, Nt(t, rt(w.transformOrigin), c, w.svgOrigin, w.smoothOrigin, !0), m = c.xOrigin, R = c.yOrigin, u.x -= c.xOffset - z.xOffset, u.y -= c.yOffset - z.yOffset), (m || R) && (g = Xt(j, !0), u.x -= m - (m * g[0] + R * g[2]), u.y -= R - (m * g[1] + R * g[3]))), L.body.removeChild(j), u.perspective || (u.perspective = z.perspective), null != w.xPercent && (u.xPercent = ot(w.xPercent, z.xPercent)), null != w.yPercent && (u.yPercent = ot(w.yPercent, z.yPercent)); else if ("object" == typeof w) {
                    if (u = {
                            scaleX: ot(null != w.scaleX ? w.scaleX : w.scale, z.scaleX),
                            scaleY: ot(null != w.scaleY ? w.scaleY : w.scale, z.scaleY),
                            scaleZ: ot(w.scaleZ, z.scaleZ),
                            x: ot(w.x, z.x),
                            y: ot(w.y, z.y),
                            z: ot(w.z, z.z),
                            xPercent: ot(w.xPercent, z.xPercent),
                            yPercent: ot(w.yPercent, z.yPercent),
                            perspective: ot(w.transformPerspective, z.perspective)
                        }, d = w.directionalRotation, null != d)if ("object" == typeof d)for (h in d)w[h] = d[h]; else w.rotation = d;
                    "string" == typeof w.x && -1 !== w.x.indexOf("%") && (u.x = 0, u.xPercent = ot(w.x, z.xPercent)), "string" == typeof w.y && -1 !== w.y.indexOf("%") && (u.y = 0, u.yPercent = ot(w.y, z.yPercent)), u.rotation = at("rotation" in w ? w.rotation : "shortRotation" in w ? w.shortRotation + "_short" : "rotationZ" in w ? w.rotationZ : z.rotation - z.skewY, z.rotation - z.skewY, "rotation", b), Ot && (u.rotationX = at("rotationX" in w ? w.rotationX : "shortRotationX" in w ? w.shortRotationX + "_short" : z.rotationX || 0, z.rotationX, "rotationX", b), u.rotationY = at("rotationY" in w ? w.rotationY : "shortRotationY" in w ? w.shortRotationY + "_short" : z.rotationY || 0, z.rotationY, "rotationY", b)), u.skewX = at(w.skewX, z.skewX - z.skewY), (u.skewY = at(w.skewY, z.skewY)) && (u.skewX += u.skewY, u.rotation += u.skewY)
                }
                for (Ot && null != w.force3D && (z.force3D = w.force3D, p = !0), z.skewType = w.skewType || z.skewType || o.defaultSkewType, f = z.force3D || z.z || z.rotationX || z.rotationY || u.z || u.rotationX || u.rotationY || u.perspective, f || null == w.scale || (u.scaleZ = 1); --x > -1;)i = Tt[x], c = u[i] - z[i], (c > y || -y > c || null != w[i] || null != I[i]) && (p = !0, s = new Rt(z, i, z[i], c, s), i in b && (s.e = b[i]), s.xs0 = 0, s.plugin = a, n._overwriteProps.push(s.n));
                return c = w.transformOrigin, z.svg && (c || w.svgOrigin) && (m = z.xOffset, R = z.yOffset, Nt(t, rt(c), u, w.svgOrigin, w.smoothOrigin), s = gt(z, "xOrigin", (_ ? z : u).xOrigin, u.xOrigin, s, T), s = gt(z, "yOrigin", (_ ? z : u).yOrigin, u.yOrigin, s, T), (m !== z.xOffset || R !== z.yOffset) && (s = gt(z, "xOffset", _ ? m : z.xOffset, z.xOffset, s, T), s = gt(z, "yOffset", _ ? R : z.yOffset, z.yOffset, s, T)), c = bt ? null : "0px 0px"), (c || Ot && f && z.zOrigin) && (zt ? (p = !0, i = kt, c = (c || Z(t, i, r, !1, "50% 50%")) + "", s = new Rt(v, i, 0, 0, s, (-1), T), s.b = v[i], s.plugin = a, Ot ? (h = z.zOrigin, c = c.split(" "), z.zOrigin = (c.length > 2 && (0 === h || "0px" !== c[2]) ? parseFloat(c[2]) : h) || 0, s.xs0 = s.e = c[0] + " " + (c[1] || "50%") + " 0px", s = new Rt(z, "zOrigin", 0, 0, s, (-1), s.n), s.b = h, s.xs0 = s.e = z.zOrigin) : s.xs0 = s.e = c) : rt(c + "", z)), p && (n._transformType = z.svg && bt || !f && 3 !== this._transformType ? 2 : 3), s
            }, prefix: !0
        }), xt("boxShadow", {
            defaultValue: "0px 0px 0px 0px #999",
            prefix: !0,
            color: !0,
            multi: !0,
            keyword: "inset"
        }), xt("borderRadius", {
            defaultValue: "0px",
            parser: function (t, e, i, s, o, a) {
                e = this.format(e);
                var l, u, h, c, f, p, d, m, R, g, _, v, y, x, w, b, T = ["borderTopLeftRadius", "borderTopRightRadius", "borderBottomRightRadius", "borderBottomLeftRadius"], z = t.style;
                for (R = parseFloat(t.offsetWidth), g = parseFloat(t.offsetHeight), l = e.split(" "), u = 0; u < T.length; u++)this.p.indexOf("border") && (T[u] = W(T[u])), f = c = Z(t, T[u], r, !1, "0px"), -1 !== f.indexOf(" ") && (c = f.split(" "), f = c[0], c = c[1]), p = h = l[u], d = parseFloat(f), v = f.substr((d + "").length), y = "=" === p.charAt(1), y ? (m = parseInt(p.charAt(0) + "1", 10), p = p.substr(2), m *= parseFloat(p), _ = p.substr((m + "").length - (0 > m ? 1 : 0)) || "") : (m = parseFloat(p), _ = p.substr((m + "").length)), "" === _ && (_ = n[i] || v), _ !== v && (x = $(t, "borderLeft", d, v), w = $(t, "borderTop", d, v), "%" === _ ? (f = x / R * 100 + "%", c = w / g * 100 + "%") : "em" === _ ? (b = $(t, "borderLeft", 1, "em"), f = x / b + "em", c = w / b + "em") : (f = x + "px", c = w + "px"), y && (p = parseFloat(f) + m + _, h = parseFloat(c) + m + _)), o = _t(z, T[u], f + " " + c, p + " " + h, !1, "0px", o);
                return o
            }, prefix: !0, formatter: pt("0px 0px 0px 0px", !1, !0)
        }), xt("borderBottomLeftRadius,borderBottomRightRadius,borderTopLeftRadius,borderTopRightRadius", {
            defaultValue: "0px",
            parser: function (t, e, i, n, s, o) {
                return _t(t.style, i, this.format(Z(t, i, r, !1, "0px 0px")), this.format(e), !1, "0px", s)
            },
            prefix: !0,
            formatter: pt("0px 0px", !1, !0)
        }), xt("backgroundPosition", {
            defaultValue: "0 0", parser: function (t, e, i, n, s, o) {
                var a, l, u, h, c, f, p = "background-position", d = r || Q(t, null), R = this.format((d ? m ? d.getPropertyValue(p + "-x") + " " + d.getPropertyValue(p + "-y") : d.getPropertyValue(p) : t.currentStyle.backgroundPositionX + " " + t.currentStyle.backgroundPositionY) || "0 0"), g = this.format(e);
                if (-1 !== R.indexOf("%") != (-1 !== g.indexOf("%")) && g.split(",").length < 2 && (f = Z(t, "backgroundImage").replace(k, ""), f && "none" !== f)) {
                    for (a = R.split(" "), l = g.split(" "), D.setAttribute("src", f), u = 2; --u > -1;)R = a[u], h = -1 !== R.indexOf("%"), h !== (-1 !== l[u].indexOf("%")) && (c = 0 === u ? t.offsetWidth - D.width : t.offsetHeight - D.height, a[u] = h ? parseFloat(R) / 100 * c + "px" : parseFloat(R) / c * 100 + "%");
                    R = a.join(" ")
                }
                return this.parseComplex(t.style, R, g, s, o)
            }, formatter: rt
        }), xt("backgroundSize", {defaultValue: "0 0", formatter: rt}), xt("perspective", {
            defaultValue: "0px",
            prefix: !0
        }), xt("perspectiveOrigin", {
            defaultValue: "50% 50%",
            prefix: !0
        }), xt("transformStyle", {prefix: !0}), xt("backfaceVisibility", {prefix: !0}), xt("userSelect", {prefix: !0}), xt("margin", {parser: dt("marginTop,marginRight,marginBottom,marginLeft")}), xt("padding", {parser: dt("paddingTop,paddingRight,paddingBottom,paddingLeft")}), xt("clip", {
            defaultValue: "rect(0px,0px,0px,0px)",
            parser: function (t, e, i, n, s, o) {
                var a, l, u;
                return 9 > m ? (l = t.currentStyle, u = 8 > m ? " " : ",", a = "rect(" + l.clipTop + u + l.clipRight + u + l.clipBottom + u + l.clipLeft + ")", e = this.format(e).split(",").join(u)) : (a = this.format(Z(t, this.p, r, !1, this.dflt)), e = this.format(e)), this.parseComplex(t.style, a, e, s, o)
            }
        }), xt("textShadow", {
            defaultValue: "0px 0px 0px #999",
            color: !0,
            multi: !0
        }), xt("autoRound,strictUnits", {
            parser: function (t, e, i, n, r) {
                return r
            }
        }), xt("border", {
            defaultValue: "0px solid #000", parser: function (t, e, i, n, s, o) {
                var a = Z(t, "borderTopWidth", r, !1, "0px"), l = this.format(e).split(" "), u = l[0].replace(y, "");
                return "px" !== u && (a = parseFloat(a) / $(t, "borderTopWidth", 1, u) + u), this.parseComplex(t.style, this.format(a + " " + Z(t, "borderTopStyle", r, !1, "solid") + " " + Z(t, "borderTopColor", r, !1, "#000")), l.join(" "), s, o)
            }, color: !0, formatter: function (t) {
                var e = t.split(" ");
                return e[0] + " " + (e[1] || "solid") + " " + (t.match(ft) || ["#000"])[0]
            }
        }), xt("borderWidth", {parser: dt("borderTopWidth,borderRightWidth,borderBottomWidth,borderLeftWidth")}), xt("float,cssFloat,styleFloat", {
            parser: function (t, e, i, n, r, s) {
                var o = t.style, a = "cssFloat" in o ? "cssFloat" : "styleFloat";
                return new Rt(o, a, 0, 0, r, (-1), i, (!1), 0, o[a], e)
            }
        });
        var Vt = function (t) {
            var e, i = this.t, n = i.filter || Z(this.data, "filter") || "", r = this.s + this.c * t | 0;
            100 === r && (-1 === n.indexOf("atrix(") && -1 === n.indexOf("radient(") && -1 === n.indexOf("oader(") ? (i.removeAttribute("filter"), e = !Z(this.data, "filter")) : (i.filter = n.replace(b, ""), e = !0)), e || (this.xn1 && (i.filter = n = n || "alpha(opacity=" + r + ")"), -1 === n.indexOf("pacity") ? 0 === r && this.xn1 || (i.filter = n + " alpha(opacity=" + r + ")") : i.filter = n.replace(x, "opacity=" + r))
        };
        xt("opacity,alpha,autoAlpha", {
            defaultValue: "1", parser: function (t, e, i, n, s, o) {
                var a = parseFloat(Z(t, "opacity", r, !1, "1")), l = t.style, u = "autoAlpha" === i;
                return "string" == typeof e && "=" === e.charAt(1) && (e = ("-" === e.charAt(0) ? -1 : 1) * parseFloat(e.substr(2)) + a), u && 1 === a && "hidden" === Z(t, "visibility", r) && 0 !== e && (a = 0), B ? s = new Rt(l, "opacity", a, e - a, s) : (s = new Rt(l, "opacity", 100 * a, 100 * (e - a), s), s.xn1 = u ? 1 : 0, l.zoom = 1, s.type = 2, s.b = "alpha(opacity=" + s.s + ")", s.e = "alpha(opacity=" + (s.s + s.c) + ")", s.data = t, s.plugin = o, s.setRatio = Vt), u && (s = new Rt(l, "visibility", 0, 0, s, (-1), null, (!1), 0, 0 !== a ? "inherit" : "hidden", 0 === e ? "hidden" : "inherit"), s.xs0 = "inherit", n._overwriteProps.push(s.n), n._overwriteProps.push(i)), s
            }
        });
        var Bt = function (t, e) {
            e && (t.removeProperty ? (("ms" === e.substr(0, 2) || "webkit" === e.substr(0, 6)) && (e = "-" + e), t.removeProperty(e.replace(z, "-$1").toLowerCase())) : t.removeAttribute(e))
        }, qt = function (t) {
            if (this.t._gsClassPT = this, 1 === t || 0 === t) {
                this.t.setAttribute("class", 0 === t ? this.b : this.e);
                for (var e = this.data, i = this.t.style; e;)e.v ? i[e.p] = e.v : Bt(i, e.p), e = e._next;
                1 === t && this.t._gsClassPT === this && (this.t._gsClassPT = null)
            } else this.t.getAttribute("class") !== this.e && this.t.setAttribute("class", this.e)
        };
        xt("className", {
            parser: function (t, e, n, s, o, a, l) {
                var u, h, c, f, p, d = t.getAttribute("class") || "", m = t.style.cssText;
                if (o = s._classNamePT = new Rt(t, n, 0, 0, o, 2), o.setRatio = qt, o.pr = -11, i = !0, o.b = d, h = J(t, r), c = t._gsClassPT) {
                    for (f = {}, p = c.data; p;)f[p.p] = 1, p = p._next;
                    c.setRatio(1)
                }
                return t._gsClassPT = o, o.e = "=" !== e.charAt(1) ? e : d.replace(new RegExp("(?:\\s|^)" + e.substr(2) + "(?![\\w-])"), "") + ("+" === e.charAt(0) ? " " + e.substr(2) : ""), t.setAttribute("class", o.e), u = tt(t, h, J(t), l, f), t.setAttribute("class", d), o.data = u.firstMPT, t.style.cssText = m, o = o.xfirst = s.parse(t, u.difs, o, a)
            }
        });
        var Ut = function (t) {
            if ((1 === t || 0 === t) && this.data._totalTime === this.data._totalDuration && "isFromStart" !== this.data.data) {
                var e, i, n, r, s, o = this.t.style, a = l.transform.parse;
                if ("all" === this.e)o.cssText = "", r = !0; else for (e = this.e.split(" ").join("").split(","), n = e.length; --n > -1;)i = e[n], l[i] && (l[i].parse === a ? r = !0 : i = "transformOrigin" === i ? kt : l[i].p), Bt(o, i);
                r && (Bt(o, zt), s = this.t._gsTransform, s && (s.svg && (this.t.removeAttribute("data-svg-origin"), this.t.removeAttribute("transform")), delete this.t._gsTransform))
            }
        };
        for (xt("clearProps", {
            parser: function (t, e, n, r, s) {
                return s = new Rt(t, n, 0, 0, s, 2), s.setRatio = Ut, s.e = e, s.pr = -10, s.data = r._tween, i = !0, s
            }
        }), u = "bezier,throwProps,physicsProps,physics2D".split(","), vt = u.length; vt--;)wt(u[vt]);
        u = o.prototype, u._firstPT = u._lastParsedTransform = u._transform = null, u._onInitTween = function (t, e, a) {
            if (!t.nodeType)return !1;
            this._target = t, this._tween = a, this._vars = e, h = e.autoRound, i = !1, n = e.suffixMap || o.suffixMap, r = Q(t, ""), s = this._overwriteProps;
            var u, p, m, R, g, _, v, y, x, b = t.style;
            if (c && "" === b.zIndex && (u = Z(t, "zIndex", r), ("auto" === u || "" === u) && this._addLazySet(b, "zIndex", 0)), "string" == typeof e && (R = b.cssText, u = J(t, r), b.cssText = R + ";" + e, u = tt(t, u, J(t)).difs, !B && w.test(e) && (u.opacity = parseFloat(RegExp.$1)), e = u, b.cssText = R), e.className ? this._firstPT = p = l.className.parse(t, e.className, "className", this, null, null, e) : this._firstPT = p = this.parse(t, e, null), this._transformType) {
                for (x = 3 === this._transformType, zt ? f && (c = !0, "" === b.zIndex && (v = Z(t, "zIndex", r), ("auto" === v || "" === v) && this._addLazySet(b, "zIndex", 0)), d && this._addLazySet(b, "WebkitBackfaceVisibility", this._vars.WebkitBackfaceVisibility || (x ? "visible" : "hidden"))) : b.zoom = 1, m = p; m && m._next;)m = m._next;
                y = new Rt(t, "transform", 0, 0, null, 2), this._linkCSSP(y, null, m), y.setRatio = zt ? Yt : Dt, y.data = this._transform || jt(t, r, !0), y.tween = a, y.pr = -1, s.pop()
            }
            if (i) {
                for (; p;) {
                    for (_ = p._next, m = R; m && m.pr > p.pr;)m = m._next;
                    (p._prev = m ? m._prev : g) ? p._prev._next = p : R = p, (p._next = m) ? m._prev = p : g = p, p = _
                }
                this._firstPT = R
            }
            return !0
        }, u.parse = function (t, e, i, s) {
            var o, a, u, c, f, p, d, m, R, g, _ = t.style;
            for (o in e)p = e[o], a = l[o], a ? i = a.parse(t, p, o, this, i, s, e) : (f = Z(t, o, r) + "", R = "string" == typeof p, "color" === o || "fill" === o || "stroke" === o || -1 !== o.indexOf("Color") || R && T.test(p) ? (R || (p = ht(p), p = (p.length > 3 ? "rgba(" : "rgb(") + p.join(",") + ")"), i = _t(_, o, f, p, !0, "transparent", i, 0, s)) : R && E.test(p) ? i = _t(_, o, f, p, !0, null, i, 0, s) : (u = parseFloat(f), d = u || 0 === u ? f.substr((u + "").length) : "", ("" === f || "auto" === f) && ("width" === o || "height" === o ? (u = nt(t, o, r), d = "px") : "left" === o || "top" === o ? (u = K(t, o, r), d = "px") : (u = "opacity" !== o ? 0 : 1, d = "")), g = R && "=" === p.charAt(1), g ? (c = parseInt(p.charAt(0) + "1", 10), p = p.substr(2), c *= parseFloat(p), m = p.replace(y, "")) : (c = parseFloat(p), m = R ? p.replace(y, "") : ""), "" === m && (m = o in n ? n[o] : d), p = c || 0 === c ? (g ? c + u : c) + m : e[o], d !== m && "" !== m && (c || 0 === c) && u && (u = $(t, o, u, d), "%" === m ? (u /= $(t, o, 100, "%") / 100, e.strictUnits !== !0 && (f = u + "%")) : "em" === m || "rem" === m || "vw" === m || "vh" === m ? u /= $(t, o, 1, m) : "px" !== m && (c = $(t, o, c, m), m = "px"), g && (c || 0 === c) && (p = c + u + m)), g && (c += u), !u && 0 !== u || !c && 0 !== c ? void 0 !== _[o] && (p || p + "" != "NaN" && null != p) ? (i = new Rt(_, o, c || u || 0, 0, i, (-1), o, (!1), 0, f, p), i.xs0 = "none" !== p || "display" !== o && -1 === o.indexOf("Style") ? p : f) : U("invalid " + o + " tween value: " + e[o]) : (i = new Rt(_, o, u, c - u, i, 0, o, h !== !1 && ("px" === m || "zIndex" === o), 0, f, p), i.xs0 = m))), s && i && !i.plugin && (i.plugin = s);
            return i
        }, u.setRatio = function (t) {
            var e, i, n, r = this._firstPT, s = 1e-6;
            if (1 !== t || this._tween._time !== this._tween._duration && 0 !== this._tween._time)if (t || this._tween._time !== this._tween._duration && 0 !== this._tween._time || this._tween._rawPrevTime === -1e-6)for (; r;) {
                if (e = r.c * t + r.s, r.r ? e = Math.round(e) : s > e && e > -s && (e = 0), r.type)if (1 === r.type)if (n = r.l, 2 === n)r.t[r.p] = r.xs0 + e + r.xs1 + r.xn1 + r.xs2; else if (3 === n)r.t[r.p] = r.xs0 + e + r.xs1 + r.xn1 + r.xs2 + r.xn2 + r.xs3; else if (4 === n)r.t[r.p] = r.xs0 + e + r.xs1 + r.xn1 + r.xs2 + r.xn2 + r.xs3 + r.xn3 + r.xs4; else if (5 === n)r.t[r.p] = r.xs0 + e + r.xs1 + r.xn1 + r.xs2 + r.xn2 + r.xs3 + r.xn3 + r.xs4 + r.xn4 + r.xs5; else {
                    for (i = r.xs0 + e + r.xs1, n = 1; n < r.l; n++)i += r["xn" + n] + r["xs" + (n + 1)];
                    r.t[r.p] = i
                } else-1 === r.type ? r.t[r.p] = r.xs0 : r.setRatio && r.setRatio(t); else r.t[r.p] = e + r.xs0;
                r = r._next
            } else for (; r;)2 !== r.type ? r.t[r.p] = r.b : r.setRatio(t), r = r._next; else for (; r;) {
                if (2 !== r.type)if (r.r && -1 !== r.type)if (e = Math.round(r.s + r.c), r.type) {
                    if (1 === r.type) {
                        for (n = r.l, i = r.xs0 + e + r.xs1, n = 1; n < r.l; n++)i += r["xn" + n] + r["xs" + (n + 1)];
                        r.t[r.p] = i
                    }
                } else r.t[r.p] = e + r.xs0; else r.t[r.p] = r.e; else r.setRatio(t);
                r = r._next
            }
        }, u._enableTransforms = function (t) {
            this._transform = this._transform || jt(this._target, r, !0), this._transformType = this._transform.svg && bt || !t && 3 !== this._transformType ? 2 : 3
        };
        var Gt = function (t) {
            this.t[this.p] = this.e, this.data._linkCSSP(this, this._next, null, !0)
        };
        u._addLazySet = function (t, e, i) {
            var n = this._firstPT = new Rt(t, e, 0, 0, this._firstPT, 2);
            n.e = i, n.setRatio = Gt, n.data = this
        }, u._linkCSSP = function (t, e, i, n) {
            return t && (e && (e._prev = t), t._next && (t._next._prev = t._prev), t._prev ? t._prev._next = t._next : this._firstPT === t && (this._firstPT = t._next, n = !0), i ? i._next = t : n || null !== this._firstPT || (this._firstPT = t), t._next = e, t._prev = i), t
        }, u._kill = function (e) {
            var i, n, r, s = e;
            if (e.autoAlpha || e.alpha) {
                s = {};
                for (n in e)s[n] = e[n];
                s.opacity = 1, s.autoAlpha && (s.visibility = 1)
            }
            return e.className && (i = this._classNamePT) && (r = i.xfirst, r && r._prev ? this._linkCSSP(r._prev, i._next, r._prev._prev) : r === this._firstPT && (this._firstPT = i._next), i._next && this._linkCSSP(i._next, i._next._next, r._prev), this._classNamePT = null), t.prototype._kill.call(this, s)
        };
        var Ht = function (t, e, i) {
            var n, r, s, o;
            if (t.slice)for (r = t.length; --r > -1;)Ht(t[r], e, i); else for (n = t.childNodes, r = n.length; --r > -1;)s = n[r], o = s.type, s.style && (e.push(J(s)), i && i.push(s)), 1 !== o && 9 !== o && 11 !== o || !s.childNodes.length || Ht(s, e, i)
        };
        return o.cascadeTo = function (t, i, n) {
            var r, s, o, a, l = e.to(t, i, n), u = [l], h = [], c = [], f = [], p = e._internals.reservedProps;
            for (t = l._targets || l.target, Ht(t, h, f), l.render(i, !0, !0), Ht(t, c), l.render(0, !0, !0), l._enabled(!0), r = f.length; --r > -1;)if (s = tt(f[r], h[r], c[r]), s.firstMPT) {
                s = s.difs;
                for (o in n)p[o] && (s[o] = n[o]);
                a = {};
                for (o in s)a[o] = h[r][o];
                u.push(e.fromTo(f[r], i, a, s))
            }
            return u
        }, t.activate([o]), o
    }, !0)
}), _gsScope._gsDefine && _gsScope._gsQueue.pop()(), function (t) {
    "use strict";
    var e = function () {
        return (_gsScope.GreenSockGlobals || _gsScope)[t]
    };
    "function" == typeof define && define.amd ? define(["../TweenLite"], e) : "undefined" != typeof module && module.exports && (require("../TweenLite.js"), module.exports = e())
}("CSSPlugin");
var _gsScope = "undefined" != typeof module && module.exports && "undefined" != typeof global ? global : this || window;
(_gsScope._gsQueue || (_gsScope._gsQueue = [])).push(function () {
    "use strict";
    var t = document.documentElement, e = window, i = function (i, n) {
        var r = "x" === n ? "Width" : "Height", s = "scroll" + r, o = "client" + r, a = document.body;
        return i === e || i === t || i === a ? Math.max(t[s], a[s]) - (e["inner" + r] || t[o] || a[o]) : i[s] - i["offset" + r]
    }, n = _gsScope._gsDefine.plugin({
        propName: "scrollTo", API: 2, version: "1.7.6", init: function (t, n, r) {
            return this._wdw = t === e, this._target = t, this._tween = r, "object" != typeof n && (n = {y: n}), this.vars = n, this._autoKill = n.autoKill !== !1, this.x = this.xPrev = this.getX(), this.y = this.yPrev = this.getY(), null != n.x ? (this._addTween(this, "x", this.x, "max" === n.x ? i(t, "x") : n.x, "scrollTo_x", !0), this._overwriteProps.push("scrollTo_x")) : this.skipX = !0, null != n.y ? (this._addTween(this, "y", this.y, "max" === n.y ? i(t, "y") : n.y, "scrollTo_y", !0), this._overwriteProps.push("scrollTo_y")) : this.skipY = !0, !0
        }, set: function (t) {
            this._super.setRatio.call(this, t);
            var n = this._wdw || !this.skipX ? this.getX() : this.xPrev, r = this._wdw || !this.skipY ? this.getY() : this.yPrev, s = r - this.yPrev, o = n - this.xPrev;
            this.x < 0 && (this.x = 0), this.y < 0 && (this.y = 0), this._autoKill && (!this.skipX && (o > 7 || o < -7) && n < i(this._target, "x") && (this.skipX = !0), !this.skipY && (s > 7 || s < -7) && r < i(this._target, "y") && (this.skipY = !0), this.skipX && this.skipY && (this._tween.kill(), this.vars.onAutoKill && this.vars.onAutoKill.apply(this.vars.onAutoKillScope || this._tween, this.vars.onAutoKillParams || []))), this._wdw ? e.scrollTo(this.skipX ? n : this.x, this.skipY ? r : this.y) : (this.skipY || (this._target.scrollTop = this.y), this.skipX || (this._target.scrollLeft = this.x)), this.xPrev = this.x, this.yPrev = this.y
        }
    }), r = n.prototype;
    n.max = i, r.getX = function () {
        return this._wdw ? null != e.pageXOffset ? e.pageXOffset : null != t.scrollLeft ? t.scrollLeft : document.body.scrollLeft : this._target.scrollLeft
    }, r.getY = function () {
        return this._wdw ? null != e.pageYOffset ? e.pageYOffset : null != t.scrollTop ? t.scrollTop : document.body.scrollTop : this._target.scrollTop
    }, r._kill = function (t) {
        return t.scrollTo_x && (this.skipX = !0), t.scrollTo_y && (this.skipY = !0), this._super._kill.call(this, t)
    }
}), _gsScope._gsDefine && _gsScope._gsQueue.pop()(), function (t, e, i, n) {
    var r = i.body || i.documentElement, r = r.style, s = "", o = "";
    "" == r.WebkitAnimation && (s = "-webkit-"), "" == r.MozAnimation && (s = "-moz-"), "" == r.OAnimation && (s = "-o-"), "" == r.WebkitTransition && (o = "-webkit-"), "" == r.MozTransition && (o = "-moz-"), "" == r.OTransition && (o = "-o-"), t.fn.extend({
        onCSSAnimationEnd: function (e) {
            var i = t(this).eq(0);
            return i.one("webkitAnimationEnd mozAnimationEnd oAnimationEnd oanimationend animationend", e), ("" != s || "animation" in r) && "0s" != i.css(s + "animation-duration") || e(), this
        }, onCSSTransitionEnd: function (e) {
            var i = t(this).eq(0);
            return i.one("webkitTransitionEnd mozTransitionEnd oTransitionEnd otransitionend transitionend", e), ("" != o || "transition" in r) && "0s" != i.css(o + "transition-duration") || e(), this
        }
    })
}(jQuery, window, document), function (t) {
    function e(t, e) {
        this.x = t, this.y = e
    }

    function i(t, e) {
        this.point = t, this.timestamp = e
    }

    function n(e) {
        return this.each(function () {
            e && t.extend(c, e);
            var i = t(this), n = {
                options: e,
                _currSpeed: 0,
                _updateSpeedTimeout: null,
                _lastMousePos: null,
                _currMousePos: null,
                _hasAttachedMousemoveEvent: !1
            };
            i.data(u, n), i.mouseenter(r), i.mouseleave(s), o.apply(this)
        })
    }

    function r() {
        var e = t(this), i = e.data(u), n = i._currMousePos, s = i._lastMousePos, o = 0;
        if (n && s) {
            var l = a(n.point, s.point);
            o = l / (n.timestamp - s.timestamp)
        }
        var h = i._currSpeed = o;
        i.options.onUpdateSpeed && i.options.onUpdateSpeed.apply(e[0], [h]), i._updateSpeedTimeout = setTimeout(function () {
            r.apply(e[0])
        }, i.options.speedPollingRate)
    }

    function s() {
        var e = t(this), i = e.data(u);
        i._currSpeed = 0, i._lastMousePosCalc = null, i._currMousePosCalc = null, i._hasAttachedMousemoveEvent = !1, i._updateSpeedTimeout && clearTimeout(i._updateSpeedTimeout)
    }

    function o() {
        var n = t(this), r = n.data(u);
        r._hasAttachedMousemoveEvent || (n.one("mousemove." + u, function (t) {
            r._currMousePos && (r._lastMousePos = r._currMousePos);
            var s = t.pageX, a = t.pageY, l = new e(s, a);
            r._currMousePos = new i(l, (new Date).getTime()), setTimeout(function () {
                o.apply(n[0])
            }, r.options.captureMouseMoveRate), r._hasAttachedMousemoveEvent = !1
        }), r._hasAttachedMousemoveEvent = !0)
    }

    function a(t, e) {
        var i = 0;
        if (t && e) {
            var n = Math.pow(t.x - e.x, 2), r = Math.pow(t.y - e.y, 2);
            i = Math.sqrt(n + r)
        }
        return i
    }

    function l() {
        return t(this).data(u)._currSpeed
    }

    var u = "cursometer", h = {init: n, getCurrentSpeed: l}, c = {
        updateSpeedRate: 20,
        captureMouseMoveRate: 15,
        onUpdateSpeed: t.noop
    };
    t.fn.cursometer = function (e) {
        return h[e] ? h[e].apply(this, Array.prototype.slice.call(arguments, 1)) : "object" != typeof e && e ? void t.error("Method " + e + " does not exist on jQuery.cursometer") : h.init.apply(this, arguments)
    }
}(jQuery), !function (t, e) {
    "object" == typeof exports && "object" == typeof module ? module.exports = e() : "function" == typeof define && define.amd ? define([], e) : "object" == typeof exports ? exports.baffle = e() : t.baffle = e()
}(this, function () {
    return function (t) {
        function e(n) {
            if (i[n])return i[n].exports;
            var r = i[n] = {exports: {}, id: n, loaded: !1};
            return t[n].call(r.exports, r, r.exports, e), r.loaded = !0, r.exports
        }

        var i = {};
        return e.m = t, e.c = i, e.p = "", e(0)
    }([function (t, e, i) {
        "use strict";
        function n(t) {
            return t && t.__esModule ? t : {"default": t}
        }

        var r = i(2), s = n(r);
        t.exports = s["default"]
    }, function (t, e) {
        "use strict";
        function i(t, e) {
            for (var i in e)e.hasOwnProperty(i) && (t[i] = e[i]);
            return t
        }

        function n(t, e) {
            return t.split("").map(e).join("")
        }

        function r(t) {
            return t[Math.floor(Math.random() * t.length)]
        }

        function s(t, e) {
            for (var i = 0, n = t.length; i < n; i++)e(t[i], i)
        }

        function o(t) {
            return t.map(function (t, e) {
                return !!t && e
            }).filter(function (t) {
                return t !== !1
            })
        }

        function a(t) {
            return "string" == typeof t ? [].slice.call(document.querySelectorAll(t)) : [NodeList, HTMLCollection].some(function (e) {
                return t instanceof e
            }) ? [].slice.call(t) : t.nodeType ? [t] : t
        }

        Object.defineProperty(e, "__esModule", {value: !0}), e.extend = i, e.mapString = n, e.sample = r, e.each = s, e.getTruthyIndices = o, e.getElements = a
    }, function (t, e, i) {
        "use strict";
        function n(t) {
            return t && t.__esModule ? t : {"default": t}
        }

        function r(t, e) {
            if (!(t instanceof e))throw new TypeError("Cannot call a class as a function")
        }

        Object.defineProperty(e, "__esModule", {value: !0});
        var s = i(1), o = i(3), a = n(o), l = {
            characters: "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz~!@#$%^&*()-+=[]{}|;:,./<>?".split(""),
            speed: 50
        }, u = function () {
            function t(e, i) {
                r(this, t), this.options = (0, s.extend)(Object.create(l), i), this.elements = (0, s.getElements)(e).map(a["default"]), this.running = !1
            }

            return t.prototype.once = function () {
                var t = this;
                return (0, s.each)(this.elements, function (e) {
                    return e.write(t.options.characters)
                }), this.running = !0, this
            }, t.prototype.start = function () {
                var t = this;
                return clearInterval(this.interval), (0, s.each)(this.elements, function (t) {
                    return t.init()
                }), this.interval = setInterval(function () {
                    return t.once()
                }, this.options.speed), this.running = !0, this
            }, t.prototype.stop = function () {
                return clearInterval(this.interval), this.running = !1, this
            }, t.prototype.set = function (t) {
                return (0, s.extend)(this.options, t), this.running && this.start(), this
            }, t.prototype.text = function (t) {
                var e = this;
                return (0, s.each)(this.elements, function (i) {
                    i.text(t(i.value)), e.running || i.write()
                }), this
            }, t.prototype.reveal = function () {
                var t = this, e = arguments.length <= 0 || void 0 === arguments[0] ? 0 : arguments[0], i = arguments.length <= 1 || void 0 === arguments[1] ? 0 : arguments[1], n = e / this.options.speed || 1, r = function () {
                    clearInterval(t.interval), t.running = !0, t.interval = setInterval(function () {
                        var e = t.elements.filter(function (t) {
                            return !t.bitmap.every(function (t) {
                                return !t
                            })
                        });
                        (0, s.each)(e, function (e) {
                            var i = Math.ceil(e.value.length / n);
                            e.decay(i).write(t.options.characters)
                        }), e.length || (t.stop(), (0, s.each)(t.elements, function (t) {
                            return t.init()
                        }))
                    }, t.options.speed)
                };
                return setTimeout(r, i), this
            }, t
        }();
        e["default"] = function (t, e) {
            return new u(t, e)
        }
    }, function (t, e, i) {
        "use strict";
        function n(t, e) {
            if (!t)throw new ReferenceError("this hasn't been initialised - super() hasn't been called");
            return !e || "object" != typeof e && "function" != typeof e ? t : e
        }

        function r(t, e) {
            if ("function" != typeof e && null !== e)throw new TypeError("Super expression must either be null or a function, not " + typeof e);
            t.prototype = Object.create(e && e.prototype, {
                constructor: {
                    value: t,
                    enumerable: !1,
                    writable: !0,
                    configurable: !0
                }
            }), e && (Object.setPrototypeOf ? Object.setPrototypeOf(t, e) : t.__proto__ = e)
        }

        function s(t, e) {
            if (!(t instanceof e))throw new TypeError("Cannot call a class as a function")
        }

        Object.defineProperty(e, "__esModule", {value: !0});
        var o = i(1), a = function () {
            function t(e) {
                s(this, t), this.value = e, this.init()
            }

            return t.prototype.init = function () {
                return this.bitmap = this.value.split("").map(function () {
                    return 1
                }), this
            }, t.prototype.render = function () {
                var t = this, e = arguments.length <= 0 || void 0 === arguments[0] ? [] : arguments[0], i = arguments.length <= 1 || void 0 === arguments[1] ? [" "] : arguments[1];
                return e.length ? (0, o.mapString)(this.value, function (n, r) {
                    return i.indexOf(n) > -1 ? n : t.bitmap[r] ? (0, o.sample)(e) : n
                }) : this.value
            }, t.prototype.decay = function () {
                for (var t = arguments.length <= 0 || void 0 === arguments[0] ? 1 : arguments[0]; t--;) {
                    var e = (0, o.getTruthyIndices)(this.bitmap);
                    this.bitmap[(0, o.sample)(e)] = 0
                }
                return this
            }, t.prototype.text = function () {
                var t = arguments.length <= 0 || void 0 === arguments[0] ? this.value : arguments[0];
                return this.value = t, this.init(), this
            }, t
        }(), l = function (t) {
            function e(i) {
                s(this, e);
                var r = n(this, t.call(this, i.textContent));
                return r.element = i, r
            }

            return r(e, t), e.prototype.write = function (t) {
                return this.element.textContent = this.render(t), this
            }, e
        }(a);
        e["default"] = function (t) {
            return new l(t)
        }
    }])
}), !function (t, e) {
    "object" == typeof exports && "object" == typeof module ? module.exports = e() : "function" == typeof define && define.amd ? define([], e) : "object" == typeof exports ? exports.inView = e() : t.inView = e()
}(this, function () {
    return function (t) {
        function e(n) {
            if (i[n])return i[n].exports;
            var r = i[n] = {exports: {}, id: n, loaded: !1};
            return t[n].call(r.exports, r, r.exports, e), r.loaded = !0, r.exports
        }

        var i = {};
        return e.m = t, e.c = i, e.p = "", e(0)
    }([function (t, e, i) {
        "use strict";
        function n(t) {
            return t && t.__esModule ? t : {"default": t}
        }

        var r = i(1), s = n(r);
        t.exports = s["default"]
    }, function (t, e, i) {
        "use strict";
        function n(t) {
            return t && t.__esModule ? t : {"default": t}
        }

        Object.defineProperty(e, "__esModule", {value: !0}), e.inViewport = void 0;
        var r = i(8), s = n(r), o = i(3), a = n(o), l = e.inViewport = function (t) {
            var e = arguments.length <= 1 || void 0 === arguments[1] ? 0 : arguments[1], i = t.getBoundingClientRect(), n = i.top, r = i.right, s = i.bottom, o = i.left;
            return s > e && r > e && window.innerWidth - o > e && window.innerHeight - n > e
        }, u = function () {
            var t = 100, e = ["scroll", "resize", "load"], i = 0, n = {history: []};
            e.forEach(function (e) {
                return addEventListener(e, (0, s["default"])(function () {
                    n.history.forEach(function (t) {
                        n[t].check(i)
                    })
                }, t))
            });
            var r = function (t) {
                if ("string" == typeof t) {
                    var e = [].slice.call(document.querySelectorAll(t));
                    return n.history.indexOf(t) > -1 ? n[t].elements = e : (n[t] = (0, a["default"])(e), n.history.push(t)), n[t]
                }
            };
            return r.offset = function (t) {
                "number" == typeof t && (i = t)
            }, r.is = l, r
        };
        e["default"] = u()
    }, function (t, e) {
        function i(t) {
            var e = typeof t;
            return !!t && ("object" == e || "function" == e)
        }

        t.exports = i
    }, function (t, e, i) {
        "use strict";
        function n(t, e) {
            if (!(t instanceof e))throw new TypeError("Cannot call a class as a function")
        }

        Object.defineProperty(e, "__esModule", {value: !0});
        var r = function () {
            function t(t, e) {
                for (var i = 0; i < e.length; i++) {
                    var n = e[i];
                    n.enumerable = n.enumerable || !1, n.configurable = !0, "value" in n && (n.writable = !0), Object.defineProperty(t, n.key, n)
                }
            }

            return function (e, i, n) {
                return i && t(e.prototype, i), n && t(e, n), e
            }
        }(), s = i(1), o = function () {
            function t(e) {
                n(this, t), this.current = [], this.elements = e, this.handlers = {
                    enter: [],
                    exit: []
                }, this.singles = {enter: [], exit: []}
            }

            return r(t, [{
                key: "check", value: function (t) {
                    var e = this;
                    return this.elements.forEach(function (i) {
                        var n = (0, s.inViewport)(i, t), r = e.current.indexOf(i), o = r > -1, a = n && !o, l = !n && o;
                        a && (e.current.push(i), e.emit("enter", i)), l && (e.current.splice(r, 1), e.emit("exit", i))
                    }), this
                }
            }, {
                key: "on", value: function (t, e) {
                    return this.handlers[t].push(e), this
                }
            }, {
                key: "once", value: function (t, e) {
                    return this.singles[t].unshift(e), this
                }
            }, {
                key: "emit", value: function (t, e) {
                    for (; this.singles[t].length;)this.singles[t].pop()(e);
                    for (var i = this.handlers[t].length; --i > -1;)this.handlers[t][i](e);
                    return this
                }
            }]), t
        }();
        e["default"] = function (t) {
            return new o(t)
        }
    }, function (t, e) {
        (function (e) {
            var i = "object" == typeof e && e && e.Object === Object && e;
            t.exports = i
        }).call(e, function () {
            return this
        }())
    }, function (t, e, i) {
        var n = i(4), r = "object" == typeof self && self && self.Object === Object && self, s = n || r || Function("return this")();
        t.exports = s
    }, function (t, e, i) {
        function n(t, e, i) {
            function n(e) {
                var i = _, n = v;
                return _ = v = void 0, T = e, x = t.apply(n, i)
            }

            function h(t) {
                return T = t, w = setTimeout(p, e), z ? n(t) : x
            }

            function c(t) {
                var i = t - b, n = t - T, r = e - i;
                return P ? u(r, y - n) : r
            }

            function f(t) {
                var i = t - b, n = t - T;
                return void 0 === b || i >= e || i < 0 || P && n >= y
            }

            function p() {
                var t = s();
                return f(t) ? d(t) : void(w = setTimeout(p, c(t)))
            }

            function d(t) {
                return w = void 0, k && _ ? n(t) : (_ = v = void 0, x)
            }

            function m() {
                void 0 !== w && clearTimeout(w), T = 0, _ = b = v = w = void 0
            }

            function R() {
                return void 0 === w ? x : d(s())
            }

            function g() {
                var t = s(), i = f(t);
                if (_ = arguments, v = this, b = t, i) {
                    if (void 0 === w)return h(b);
                    if (P)return w = setTimeout(p, e), n(b)
                }
                return void 0 === w && (w = setTimeout(p, e)), x
            }

            var _, v, y, x, w, b, T = 0, z = !1, P = !1, k = !0;
            if ("function" != typeof t)throw new TypeError(a);
            return e = o(e) || 0, r(i) && (z = !!i.leading, P = "maxWait" in i, y = P ? l(o(i.maxWait) || 0, e) : y, k = "trailing" in i ? !!i.trailing : k), g.cancel = m, g.flush = R, g
        }

        var r = i(2), s = i(7), o = i(9), a = "Expected a function", l = Math.max, u = Math.min;
        t.exports = n
    }, function (t, e, i) {
        var n = i(5), r = function () {
            return n.Date.now()
        };
        t.exports = r
    }, function (t, e, i) {
        function n(t, e, i) {
            var n = !0, a = !0;
            if ("function" != typeof t)throw new TypeError(o);
            return s(i) && (n = "leading" in i ? !!i.leading : n, a = "trailing" in i ? !!i.trailing : a), r(t, e, {
                leading: n,
                maxWait: e,
                trailing: a
            })
        }

        var r = i(6), s = i(2), o = "Expected a function";
        t.exports = n
    }, function (t, e) {
        function i(t) {
            return t
        }

        t.exports = i
    }])
}), !function (t, e) {
    function i(i) {
        if ("undefined" == typeof i)throw new Error('Pathformer [constructor]: "element" parameter is required');
        if (i.constructor === String && (i = e.getElementById(i), !i))throw new Error('Pathformer [constructor]: "element" parameter is not related to an existing ID');
        if (!(i.constructor instanceof t.SVGElement || /^svg$/i.test(i.nodeName)))throw new Error('Pathformer [constructor]: "element" parameter must be a string or a SVGelement');
        this.el = i, this.scan(i)
    }

    function n(t, e, i) {
        this.isReady = !1, this.setElement(t, e), this.setOptions(e), this.setCallback(i), this.isReady && this.init()
    }

    i.prototype.TYPES = ["line", "ellipse", "circle", "polygon", "polyline", "rect"], i.prototype.ATTR_WATCH = ["cx", "cy", "points", "r", "rx", "ry", "x", "x1", "x2", "y", "y1", "y2"], i.prototype.scan = function (t) {
        for (var e, i, n, r, s = t.querySelectorAll(this.TYPES.join(",")), o = 0; o < s.length; o++)i = s[o], e = this[i.tagName.toLowerCase() + "ToPath"], n = e(this.parseAttr(i.attributes)), r = this.pathMaker(i, n), i.parentNode.replaceChild(r, i)
    }, i.prototype.lineToPath = function (t) {
        var e = {};
        return e.d = "M" + t.x1 + "," + t.y1 + "L" + t.x2 + "," + t.y2, e
    }, i.prototype.rectToPath = function (t) {
        var e = {}, i = parseFloat(t.x) || 0, n = parseFloat(t.y) || 0, r = parseFloat(t.width) || 0, s = parseFloat(t.height) || 0;
        return e.d = "M" + i + " " + n + " ", e.d += "L" + (i + r) + " " + n + " ", e.d += "L" + (i + r) + " " + (n + s) + " ", e.d += "L" + i + " " + (n + s) + " Z", e
    }, i.prototype.polylineToPath = function (t) {
        var e, i, n = {}, r = t.points.trim().split(" ");
        if (-1 === t.points.indexOf(",")) {
            var s = [];
            for (e = 0; e < r.length; e += 2)s.push(r[e] + "," + r[e + 1]);
            r = s
        }
        for (i = "M" + r[0], e = 1; e < r.length; e++)-1 !== r[e].indexOf(",") && (i += "L" + r[e]);
        return n.d = i, n
    }, i.prototype.polygonToPath = function (t) {
        var e = i.prototype.polylineToPath(t);
        return e.d += "Z", e
    }, i.prototype.ellipseToPath = function (t) {
        var e = t.cx - t.rx, i = t.cy, n = parseFloat(t.cx) + parseFloat(t.rx), r = t.cy, s = {};
        return s.d = "M" + e + "," + i + "A" + t.rx + "," + t.ry + " 0,1,1 " + n + "," + r + "A" + t.rx + "," + t.ry + " 0,1,1 " + e + "," + r, s
    }, i.prototype.circleToPath = function (t) {
        var e = {}, i = t.cx - t.r, n = t.cy, r = parseFloat(t.cx) + parseFloat(t.r), s = t.cy;
        return e.d = "M" + i + "," + n + "A" + t.r + "," + t.r + " 0,1,1 " + r + "," + s + "A" + t.r + "," + t.r + " 0,1,1 " + i + "," + s, e
    }, i.prototype.pathMaker = function (t, i) {
        var n, r, s = e.createElementNS("http://www.w3.org/2000/svg", "path");
        for (n = 0; n < t.attributes.length; n++)r = t.attributes[n], -1 === this.ATTR_WATCH.indexOf(r.name) && s.setAttribute(r.name, r.value);
        for (n in i)s.setAttribute(n, i[n]);
        return s
    }, i.prototype.parseAttr = function (t) {
        for (var e, i = {}, n = 0; n < t.length; n++) {
            if (e = t[n], -1 !== this.ATTR_WATCH.indexOf(e.name) && -1 !== e.value.indexOf("%"))throw new Error("Pathformer [parseAttr]: a SVG shape got values in percentage. This cannot be transformed into 'path' tags. Please use 'viewBox'.");
            i[e.name] = e.value
        }
        return i
    };
    var r, s, o;
    n.LINEAR = function (t) {
        return t
    }, n.EASE = function (t) {
        return -Math.cos(t * Math.PI) / 2 + .5
    }, n.EASE_OUT = function (t) {
        return 1 - Math.pow(1 - t, 3)
    }, n.EASE_IN = function (t) {
        return Math.pow(t, 3)
    }, n.EASE_OUT_BOUNCE = function (t) {
        var e = -Math.cos(.5 * t * Math.PI) + 1, i = Math.pow(e, 1.5), n = Math.pow(1 - t, 2), r = -Math.abs(Math.cos(2.5 * i * Math.PI)) + 1;
        return 1 - n + r * n
    }, n.prototype.setElement = function (i, n) {
        if ("undefined" == typeof i)throw new Error('Vivus [constructor]: "element" parameter is required');
        if (i.constructor === String && (i = e.getElementById(i), !i))throw new Error('Vivus [constructor]: "element" parameter is not related to an existing ID');
        if (this.parentEl = i, n && n.file) {
            var r = e.createElement("object");
            r.setAttribute("type", "image/svg+xml"), r.setAttribute("data", n.file), r.setAttribute("built-by-vivus", "true"), i.appendChild(r), i = r
        }
        switch (i.constructor) {
            case t.SVGSVGElement:
            case t.SVGElement:
                this.el = i, this.isReady = !0;
                break;
            case t.HTMLObjectElement:
                var s, o;
                o = this, s = function (t) {
                    if (!o.isReady) {
                        if (o.el = i.contentDocument && i.contentDocument.querySelector("svg"), !o.el && t)throw new Error("Vivus [constructor]: object loaded does not contain any SVG");
                        return o.el ? (i.getAttribute("built-by-vivus") && (o.parentEl.insertBefore(o.el, i), o.parentEl.removeChild(i), o.el.setAttribute("width", "100%"), o.el.setAttribute("height", "100%")), o.isReady = !0, o.init(), !0) : void 0
                    }
                }, s() || i.addEventListener("load", s);
                break;
            default:
                throw new Error('Vivus [constructor]: "element" parameter is not valid (or miss the "file" attribute)')
        }
    }, n.prototype.setOptions = function (e) {
        var i = ["delayed", "async", "oneByOne", "scenario", "scenario-sync"], r = ["inViewport", "manual", "autostart"];
        if (void 0 !== e && e.constructor !== Object)throw new Error('Vivus [constructor]: "options" parameter must be an object');
        if (e = e || {}, e.type && -1 === i.indexOf(e.type))throw new Error("Vivus [constructor]: " + e.type + " is not an existing animation `type`");
        if (this.type = e.type || i[0], e.start && -1 === r.indexOf(e.start))throw new Error("Vivus [constructor]: " + e.start + " is not an existing `start` option");
        if (this.start = e.start || r[0], this.isIE = -1 !== t.navigator.userAgent.indexOf("MSIE") || -1 !== t.navigator.userAgent.indexOf("Trident/") || -1 !== t.navigator.userAgent.indexOf("Edge/"), this.duration = o(e.duration, 120), this.delay = o(e.delay, null), this.dashGap = o(e.dashGap, 1), this.forceRender = e.hasOwnProperty("forceRender") ? !!e.forceRender : this.isIE, this.selfDestroy = !!e.selfDestroy, this.onReady = e.onReady, this.frameLength = this.currentFrame = this.map = this.delayUnit = this.speed = this.handle = null, this.ignoreInvisible = !!e.hasOwnProperty("ignoreInvisible") && !!e.ignoreInvisible, this.animTimingFunction = e.animTimingFunction || n.LINEAR, this.pathTimingFunction = e.pathTimingFunction || n.LINEAR, this.delay >= this.duration)throw new Error("Vivus [constructor]: delay must be shorter than duration")
    }, n.prototype.setCallback = function (t) {
        if (t && t.constructor !== Function)throw new Error('Vivus [constructor]: "callback" parameter must be a function');
        this.callback = t || function () {
            }
    }, n.prototype.mapping = function () {
        var e, i, n, r, s, a, l, u;
        for (u = a = l = 0, i = this.el.querySelectorAll("path"), e = 0; e < i.length; e++)n = i[e], this.isInvisible(n) || (s = {
            el: n,
            length: Math.ceil(n.getTotalLength())
        }, isNaN(s.length) ? t.console && console.warn && console.warn("Vivus [mapping]: cannot retrieve a path element length", n) : (this.map.push(s), n.style.strokeDasharray = s.length + " " + (s.length + 2 * this.dashGap), n.style.strokeDashoffset = s.length + this.dashGap, s.length += this.dashGap, a += s.length, this.renderPath(e)));
        for (a = 0 === a ? 1 : a, this.delay = null === this.delay ? this.duration / 3 : this.delay, this.delayUnit = this.delay / (i.length > 1 ? i.length - 1 : 1),
                 e = 0; e < this.map.length; e++) {
            switch (s = this.map[e], this.type) {
                case"delayed":
                    s.startAt = this.delayUnit * e, s.duration = this.duration - this.delay;
                    break;
                case"oneByOne":
                    s.startAt = l / a * this.duration, s.duration = s.length / a * this.duration;
                    break;
                case"async":
                    s.startAt = 0, s.duration = this.duration;
                    break;
                case"scenario-sync":
                    n = s.el, r = this.parseAttr(n), s.startAt = u + (o(r["data-delay"], this.delayUnit) || 0), s.duration = o(r["data-duration"], this.duration), u = void 0 !== r["data-async"] ? s.startAt : s.startAt + s.duration, this.frameLength = Math.max(this.frameLength, s.startAt + s.duration);
                    break;
                case"scenario":
                    n = s.el, r = this.parseAttr(n), s.startAt = o(r["data-start"], this.delayUnit) || 0, s.duration = o(r["data-duration"], this.duration), this.frameLength = Math.max(this.frameLength, s.startAt + s.duration)
            }
            l += s.length, this.frameLength = this.frameLength || this.duration
        }
    }, n.prototype.drawer = function () {
        var t = this;
        this.currentFrame += this.speed, this.currentFrame <= 0 ? (this.stop(), this.reset(), this.callback(this)) : this.currentFrame >= this.frameLength ? (this.stop(), this.currentFrame = this.frameLength, this.trace(), this.selfDestroy && this.destroy(), this.callback(this)) : (this.trace(), this.handle = r(function () {
            t.drawer()
        }))
    }, n.prototype.trace = function () {
        var t, e, i, n;
        for (n = this.animTimingFunction(this.currentFrame / this.frameLength) * this.frameLength, t = 0; t < this.map.length; t++)i = this.map[t], e = (n - i.startAt) / i.duration, e = this.pathTimingFunction(Math.max(0, Math.min(1, e))), i.progress !== e && (i.progress = e, i.el.style.strokeDashoffset = Math.floor(i.length * (1 - e)), this.renderPath(t))
    }, n.prototype.renderPath = function (t) {
        if (this.forceRender && this.map && this.map[t]) {
            var e = this.map[t], i = e.el.cloneNode(!0);
            e.el.parentNode.replaceChild(i, e.el), e.el = i
        }
    }, n.prototype.init = function () {
        this.frameLength = 0, this.currentFrame = 0, this.map = [], new i(this.el), this.mapping(), this.starter(), this.onReady && this.onReady(this)
    }, n.prototype.starter = function () {
        switch (this.start) {
            case"manual":
                return;
            case"autostart":
                this.play();
                break;
            case"inViewport":
                var e = this, i = function () {
                    e.isInViewport(e.parentEl, 1) && (e.play(), t.removeEventListener("scroll", i))
                };
                t.addEventListener("scroll", i), i()
        }
    }, n.prototype.getStatus = function () {
        return 0 === this.currentFrame ? "start" : this.currentFrame === this.frameLength ? "end" : "progress"
    }, n.prototype.reset = function () {
        return this.setFrameProgress(0)
    }, n.prototype.finish = function () {
        return this.setFrameProgress(1)
    }, n.prototype.setFrameProgress = function (t) {
        return t = Math.min(1, Math.max(0, t)), this.currentFrame = Math.round(this.frameLength * t), this.trace(), this
    }, n.prototype.play = function (t) {
        if (t && "number" != typeof t)throw new Error("Vivus [play]: invalid speed");
        return this.speed = t || 1, this.handle || this.drawer(), this
    }, n.prototype.stop = function () {
        return this.handle && (s(this.handle), this.handle = null), this
    }, n.prototype.destroy = function () {
        this.stop();
        var t, e;
        for (t = 0; t < this.map.length; t++)e = this.map[t], e.el.style.strokeDashoffset = null, e.el.style.strokeDasharray = null, this.renderPath(t)
    }, n.prototype.isInvisible = function (t) {
        var e, i = t.getAttribute("data-ignore");
        return null !== i ? "false" !== i : !!this.ignoreInvisible && (e = t.getBoundingClientRect(), !e.width && !e.height)
    }, n.prototype.parseAttr = function (t) {
        var e, i = {};
        if (t && t.attributes)for (var n = 0; n < t.attributes.length; n++)e = t.attributes[n], i[e.name] = e.value;
        return i
    }, n.prototype.isInViewport = function (t, e) {
        var i = this.scrollY(), n = i + this.getViewportH(), r = t.getBoundingClientRect(), s = r.height, o = i + r.top, a = o + s;
        return e = e || 0, n >= o + s * e && a >= i
    }, n.prototype.docElem = t.document.documentElement, n.prototype.getViewportH = function () {
        var e = this.docElem.clientHeight, i = t.innerHeight;
        return i > e ? i : e
    }, n.prototype.scrollY = function () {
        return t.pageYOffset || this.docElem.scrollTop
    }, r = function () {
        return t.requestAnimationFrame || t.webkitRequestAnimationFrame || t.mozRequestAnimationFrame || t.oRequestAnimationFrame || t.msRequestAnimationFrame || function (e) {
                return t.setTimeout(e, 1e3 / 60)
            }
    }(), s = function () {
        return t.cancelAnimationFrame || t.webkitCancelAnimationFrame || t.mozCancelAnimationFrame || t.oCancelAnimationFrame || t.msCancelAnimationFrame || function (e) {
                return t.clearTimeout(e)
            }
    }(), o = function (t, e) {
        var i = parseInt(t, 10);
        return i >= 0 ? i : e
    }, "function" == typeof define && define.amd ? define([], function () {
        return n
    }) : "object" == typeof exports ? module.exports = n : t.Vivus = n
}(window, document);
var Util = function (t) {
    "use strict";
    var e = t(".main-navigation"), i = function (t, e, i) {
        var n;
        return function () {
            var r = this, s = arguments, o = function () {
                n = null, i || t.apply(r, s)
            }, a = i && !n;
            clearTimeout(n), n = setTimeout(o, e), a && t.apply(r, s)
        }
    }, n = function () {
        t("img").on("dragstart", function (t) {
            t.preventDefault()
        })
    }, r = function () {
        t(document).on("click", "#all-works", function (i) {
            i.preventDefault(), e.addClass("is-hidden"), t("body").addClass("is-works-open overflow"), Util.sendGAEvent("Case Study", "Open Index Case Studies", !1)
        })
    }, s = function () {
        t(document).on("click", ".works-index-close", function (i) {
            i.preventDefault(), e.removeClass("is-hidden"), t("body").removeClass("is-works-open overflow")
        })
    }, o = function () {
        t(document).on("click", ".open-contact", function (e) {
            e.preventDefault(), t("body").addClass("is-contact-open"), Util.sendGAEvent("Contact", "Open Contact Section", !1), setTimeout(function () {
                t("body").addClass("overflow")
            }, 1200)
        })
    }, a = function () {
        t(document).on("click", ".close-contact", function (e) {
            e.preventDefault(), t("body").removeClass("is-contact-open"), setTimeout(function () {
                t("body").removeClass("overflow")
            }, 1200)
        })
    }, l = function () {
        t(document).on("click", ".case-study", function (t) {
            Util.sendGAEvent("Case Study", "Click on Home Case Study", t.currentTarget.id)
        })
    }, u = function () {
        //var e = t(document).find("title").text(), i = "Your title";
       // document.addEventListener("visibilitychange", function () {
           // document.hidden ? t(document).find("title").text(i) : t(document).find("title").text(e)
        //})
    }, h = function (t) {
        ga("set", "page", t), ga("send", "pageview")
    }, c = function (t, e, i) {
        i ? ga("send", "event", t, e, i) : ga("send", "event", t, e)
    }, f = function () {

    }, p = function () {
        o(), a(), r(), s(), n(), l()
    };
    return {debounce: i, sendGAPageView: h, sendGAEvent: c, addConsoleCopy: f, onPageVisibilityChange: u, init: p}
}(jQuery), Navigation = function (t) {
    "use strict";
    var e = function () {
        function e() {
            var e = t(window).scrollTop();
            i(e), s = e, r = !1
        }

        function i(t) {
            s - t > o ? (n.removeClass("is-hidden").addClass("view-in-scroll"), t < 20 && n.removeClass("view-in-scroll")) : t - s > o && t > a && n.addClass("is-hidden")
        }

        var n = t(".main-navigation"), r = !1, s = 0, o = 10, a = 150;
        t(window).on("scroll", function () {
            r || (r = !0, window.requestAnimationFrame ? requestAnimationFrame(e) : setTimeout(e, 250))
        })
    }, i = function () {
        t(document).on("click", "a[data-scroll]", function (e) {
            e.preventDefault();
            var i = t(this).attr("href"), n = t(i).offset().top;
            TweenLite.to(window, 1.2, {scrollTo: {y: n}, ease: Power4.easeInOut})
        })
    }, n = function () {
        var e = t(".no-single-work"), i = t(".in-single-work");
        t(".single-work").length || t(".privacy").length ? (e.addClass("hidden"), i.addClass("not-hidden")) : (e.removeClass("hidden"), i.removeClass("not-hidden"))
    }, r = function (t) {
        var e = t.find(".no-single-work"), i = t.find(".in-single-work");
        t.find(".single-work").length ? (e.addClass("hidden"), i.addClass("not-hidden")) : (e.removeClass("hidden"), i.removeClass("not-hidden"))
    }, s = function () {
        t(document).on("click", ".menu-trigger", function () {
            t("body").addClass("is-mobile-menu-open")
        }), t(document).on("click", ".close-trigger", function () {
            t("body").removeClass("is-mobile-menu-open")
        }), t(document).on("click", ".mobile-nav-links a", function () {
            t("body").removeClass("is-mobile-menu-open")
        })
    }, o = function () {
        e(), i(), n(), s()
    };
    return {init: o, manageChangePageNavLinks: r}
}(jQuery), Form = function (t) {
    "use strict";
    var e = function (t) {
        return !!/[0-9\-\.\_a-z]+@[0-9\-\.a-z]+\.[a-z]+/.test(t)
    }, i = function () {
        var i = t("#contact-form"), n = t("#name"), r = t("#email"), s = t("#message"), o = i.find(".submit-button");
        i.find("input, textarea").on("keyup", function () {
            "" !== n.val() && "" !== r.val() && e(r.val()) && "" !== s.val() ? o.prop("disabled", !1) : o.prop("disabled", !0)
        })
    }, n = function () {
        var e = t("#contact-half"), i = t("#contact-form"), n = i.attr("action"), r = i.find(".submit-button"), s = t(".form-message");
        i.on("submit", function (o) {
            o.preventDefault();
            var a = i.serialize();
            Util.sendGAEvent("Contact", "Try Contact me", !1), e.addClass("is-form-processing"), t.ajax({
                type: "POST",
                url: n,
                data: a
            }).done(function (t) {
                s.text(t), setTimeout(function () {
                    e.removeClass("error").addClass("success")
                }, 2e3), setTimeout(function () {
                    e.removeClass("is-form-processing")
                }, 6e3), setTimeout(function () {
                    e.removeClass("success"), r.prop("disabled", !0)
                }, 7e3), Util.sendGAEvent("Contact", "Contact Success", !1), i.find("input, textarea").val("")
            }).fail(function (t) {
                "" !== t.responseText ? s.text(t.responseText) : s.text("An error occured and your message could not be sent."), setTimeout(function () {
                    e.removeClass("success").addClass("error")
                }, 2e3), setTimeout(function () {
                    e.removeClass("is-form-processing")
                }, 6e3), setTimeout(function () {
                    e.removeClass("error")
                }, 7e3), Util.sendGAEvent("Contact", "Contact Error", !1)
            })
        })
    }, r = function () {
        n(), i()
    };
    return {init: r}
}(jQuery), Cookie = function (t) {
    "use strict";
    var e = t(".cookie-banner"), i = e.find(".confirm-cookies"), n = function (t, e, i) {
        var n;
        if (i) {
            var r = new Date;
            r.setTime(r.getTime() + 24 * i * 60 * 60 * 1e3), n = "; expires=" + r.toGMTString()
        } else n = "";
        document.cookie = encodeURIComponent(t) + "=" + encodeURIComponent(e) + n + "; path=/"
    }, r = function (t) {
        for (var e = encodeURIComponent(t) + "=", i = document.cookie.split(";"), n = 0; n < i.length; n++) {
            for (var r = i[n]; " " === r.charAt(0);)r = r.substring(1, r.length);
            if (0 === r.indexOf(e))return decodeURIComponent(r.substring(e.length, r.length))
        }
        return null
    }, s = function (t) {
        n(t, "", -1)
    }, o = function () {
        i.on("click", function (t) {
            t.preventDefault(), e.removeClass("is-banner-open"), n("banner", "active", 30)
        })
    };
    return {init: o, create: n, read: r, destroy: s}
}(jQuery), AnimationManager = function (t) {
    "use strict";
    var e = function (e) {
        var i = t("#page-transition"), n = e.data("destination");
        i.removeClass("home cerasa lato la-francesca sportland").addClass(n)
    }, i = function () {
        t(document).on("click", "#change-theme", function (e) {
            e.preventDefault(), t("body").hasClass("bright-theme") ? (t("body").removeClass("bright-theme"), Cookie.create("theme", "default", 30)) : (t("body").addClass("bright-theme"), Cookie.create("theme", "bright", 30))
        })
    }, n = function () {
        "bright" == Cookie.read("theme") && t("body").length ? t("body").addClass("bright-theme") : t("body").removeClass("bright-theme")
    }, r = function () {
        var e = t(".main-navigation");
        e.addClass("is-loaded")
    }, s = function () {
        var e = t("body");
        e.length && e.addClass("is-loaded")
    }, o = function () {
        var e = t(".context-stripe-focus-area");
        e.length && (e.addClass("is-loaded"), Modernizr.mq("(min-width: 1024px)") && window.baffle(document.querySelectorAll(".stripe-baffle"), {characters: "█▓▒░█▓▒░█▓▒░<>/"}).start().reveal(1e3, 800))
    }, a = function () {
        var e = t(".text-loading-overlay");
        e.length && (e.addClass("is-reveal"), e.onCSSTransitionEnd(function () {
            e.removeAttr("style")
        }))
    }, l = function () {
        Modernizr.mq("(min-width: 1024px)") && new Vivus("logo-anim", {duration: 55})
    }, u = function () {
        if (t(".main-hero-subtitle").length) {
            var e = window.baffle(document.querySelector(".main-hero-subtitle"), {characters: "█▓▒░█▓▒░█▓▒░<>/"});
            Modernizr.mq("(min-width: 1024px)") && setTimeout(function () {
                e.start().reveal(700, 800)
            }, 200)
        }
    }, h = function (i, n) {
        e(i), setTimeout(function () {
            t("body").removeClass("is-works-open overflow is-mobile-menu-open")
        }, 1300)
    }, c = function (t, e) {
        e.restartCSSAnimations()
    }, f = function (e, i) {
        var n = t("#page-transition"), r = n.find(".page-transition-content"), s = t("#page-transition-after"), o = t(window).scrollTop();
        t("body").removeClass("bright-theme"), r.removeClass("is-hidden"), TweenLite.to(n, .95, {
            x: "100%",
            ease: Power4.easeInOut,
            delay: .09,
            onComplete: function () {
                Navigation.manageChangePageNavLinks(i), o > 0 && TweenLite.to(window, .05, {
                    scrollTo: {y: 0},
                    delay: 1,
                    ease: Power4.easeInOut
                })
            }
        }), TweenLite.to(s, .95, {x: "100%", ease: Power4.easeInOut}), setTimeout(function () {
            e.html(i)
        }, 1350)
    }, p = function (e, n) {
        function l() {
            null != Cookie.read("banner") ? p.removeClass("is-banner-open") : setTimeout(function () {
                p.addClass("is-banner-open")
            }, 1200)
        }

        var h = t("#page-transition"), c = h.find(".page-transition-content"), f = t("#page-transition-after"), p = n.find(".cookie-banner");
        setTimeout(function () {
            c.addClass("is-hidden")
        }, 2300), setTimeout(function () {
            TweenLite.to(h, 1, {
                x: "100%", ease: Power4.easeInOut, onComplete: function () {
                    Util.init(), Util.onPageVisibilityChange(), Navigation.init(), Form.init(), Cookie.init(), AnimationManager.init(), i(), t("body").length && AnimationManager.animateHeroShapes(), u(), r(), a(), s(), o(), l()
                }
            }), TweenLite.to(f, 1, {x: "100%", ease: Power4.easeInOut, delay: .09})
        }, 2500)
    }, d = function () {
        function t(t) {
            i = t.pageX / o, n = t.pageY / o, e()
        }

        function e() {
            TweenLite.to(".section-one .col-right .bottom, .section-3 .container .line, .section-3 .container .box, footer .container .line", r, {y: n-40, x: i-80, ease: Power4.easeOut}), TweenLite.to(".section-one .col-right .top, footer .container .box", s, {                
								x: i-60,
                y: n,
                ease: Power4.easeOut								
            })						
        }

        var i, n, r = 1, s = 2, o = 13, l = 20, a = document.querySelector("body");
        a.addEventListener("mousemove", t, !1)
    }, m = function () {
        Modernizr.mq("(min-width: 1024px)") && t(".about-contact").cursometer({
            onUpdateSpeed: function (e) {
                t(".about-speed").html("SP: " + e)
            }, updateSpeedRate: 20
        })
    }, R = function () {
        if (t(".main-404").length) {
            var e = function (t, e, i) {
                for (var n = 0, r = 0; r < t.length; r++)t[r] = t[r].replace(/ /g, "&nbsp;"), t[r] = "<pre>" + t[r] + "</pre>";
                i.innerHTML = t[0], n++, this.animation = setInterval(function () {
                    i.innerHTML = t[n], n++, n >= t.length && (n = 0)
                }, e), this.getCurrentFrame = function () {
                    return n
                }
            }, i = document.createElement("div"), n = document.createElement("div"), r = document.getElementById("happy-place");
            r.appendChild(i);
            var s = document.getElementById("go-home");
            s.appendChild(n), i.className = "happy", n.className = "go-home-anim";
            var o = [".(^-^)'", "-(^-^)-", "'(^-^).", "-(^o^)-", ".(^-^)'", "-(^-^)-", "'(^-^).", "-(^-^)-"], a = (new e(o, 100, i), ["[>    ]", "[>>   ]", "[>>>  ]", "[ >>> ]", "[  >>>]", "[   >>]", "[    >]", "[     ]"]);
            new e(a, 50, n);
            e.prototype.stopAnimation = function () {
                clearInterval(this.animation)
            }
        }
    }, g = function () {
        function e() {
            c.addClass("is-hidden"), p.addClass("is-hidden")
        }

        function i() {
            u(), r(), a(), s(), o()
        }

        function h() {
            null != Cookie.read("banner") ? d.removeClass("is-banner-open") : setTimeout(function () {
                d.addClass("is-banner-open")
            }, 1200)
        }

        var c = t("#loader"), f = c.find(".loader-content"), p = t("#loader-after"), d = t(".cookie-banner");
        f.addClass("is-hidden"), t("body").addClass("loaded"), n(), TweenLite.to(p, .67, {
            x: "100%",
            ease: Power4.easeInOut,
            onComplete: function () {
                setTimeout(function () {
                    e(), h(), t("#logo-anim").length && l(), setTimeout(function () {
                        i()
                    }, 300)
                }, 750)
            }
        })
    }, _ = function () {
        if (Modernizr.mq("(min-width: 1024px)")) {
            var e = t(".experiments-mask-reveal"), i = e.find("span:first"), n = e.find("span:last");
            t(".case-study").addClass("is-hidden"), t("body-text-mask").addClass("is-hidden"), t(".single-work-intro-title").addClass("is-hidden"), t(".single-work-anim-text").addClass("is-hidden"), t(".single-work-first-anim-blocks").addClass("is-hidden"), t(".color-palette-container").addClass("is-hidden"), TweenLite.to(i, 0, {
                y: "0",
                ease: Power4.easeInOut
            }), TweenLite.to(n, 0, {y: "0", ease: Power4.easeInOut})
        }
    }, v = function () {
        if (Modernizr.mq("(min-width: 1024px)")) {
            var e = t(".experiments-mask-reveal"), i = e.find("span:first"), n = e.find("span:last");
            inView.offset(230), inView(".case-study").on("enter", function (e) {
                t(e).removeClass("is-hidden")
            }), inView("body-text-mask").on("enter", function (e) {
                t(e).removeClass("is-hidden")
            }), inView(".single-work-anim-text").on("enter", function (e) {
                t(e).removeClass("is-hidden")
            }), inView(".single-work-first-anim-blocks").on("enter", function (e) {
                t(e).removeClass("is-hidden")
            }), inView(".color-palette-container").on("enter", function (e) {
                t(e).removeClass("is-hidden")
            }), inView(".experiments-mask-reveal").on("enter", function (t) {
                TweenLite.to(i, .6, {y: "-100%", ease: Power4.easeInOut}), TweenLite.to(n, .6, {
                    y: "-100%",
                    delay: .09,
                    ease: Power4.easeInOut
                })
            }), inView(".single-work-intro-title").on("enter", function (e) {
                t(e).removeClass("is-hidden")
            })
        }
    }, y = function () {
        m(), _(), v(), R(), i(), t(window).on("resize", function () {
            Util.debounce(m(), 100)
        })
    };
    return {
        init: y,
        startLoadAnimation: g,
        manageOnBefore: h,
        manageOnStart: c,
        manageOnReady: f,
        manageOnAfter: p,
        animateHeroShapes: d
    }
}(jQuery), Application = function (t) {
    "use strict";
    var e = function () {
        document.documentElement.className = "js", t(document).on("ready", function () {
            Util.init(), Util.onPageVisibilityChange(), Util.addConsoleCopy(), Navigation.init(), Form.init(), Cookie.init(), AnimationManager.init(), t("body").length && AnimationManager.animateHeroShapes()
        }), t(window).on("load", function () {
            AnimationManager.startLoadAnimation()
        });
        var e = t("#page-wrap").smoothState({
            prefetch: !0,
            cacheLength: 4,
            blacklist: ".no-smoothState",
            scroll: !1,
            onBefore: function (t, e) {
                AnimationManager.manageOnBefore(t, e)
            },
            onStart: {
                duration: 250, render: function (t) {
                    AnimationManager.manageOnStart(t, e)
                }
            },
            onProgress: {
                duration: 250, render: function (t) {
                }
            },
            onReady: {
                duration: 0, render: function (t, e) {
                    AnimationManager.manageOnReady(t, e)
                }
            },
            onAfter: function (t, e) {
                AnimationManager.manageOnAfter(t, e), Util.sendGAPageView(t.smoothState().data("smoothState").href)
            }
        }).data("smoothState")
    };
    return {start: e}
}(jQuery);
Application.start();