/** layui-v2.4.5 MIT License By https://www.layui.com */ ;
layui.define("jquery", function(e) {
	"use strict";
	var i = layui.jquery,
		o = {
			config: {},
			index: layui.colorpicker ? layui.colorpicker.index + 1e4 : 0,
			set: function(e) {
				var o = this;
				return o.config = i.extend({}, o.config, e), o
			},
			on: function(e, i) {
				return layui.onevent.call(this, "colorpicker", e, i)
			}
		},
		r = function() {
			var e = this,
				i = e.config;
			return {
				config: i
			}
		},
		t = "colorpicker",
		n = "layui-show",
		l = "layui-colorpicker",
		c = ".layui-colorpicker-main",
		a = "layui-icon-down",
		s = "layui-icon-close",
		f = "layui-colorpicker-trigger-span",
		d = "layui-colorpicker-trigger-i",
		u = "layui-colorpicker-side",
		p = "layui-colorpicker-side-slider",
		g = "layui-colorpicker-basis",
		v = "layui-colorpicker-alpha-bgcolor",
		h = "layui-colorpicker-alpha-slider",
		m = "layui-colorpicker-basis-cursor",
		b = "layui-colorpicker-main-input",
		k = function(e) {
			var i = {
					h: 0,
					s: 0,
					b: 0
				},
				o = Math.min(e.r, e.g, e.b),
				r = Math.max(e.r, e.g, e.b),
				t = r - o;
			return i.b = r, i.s = 0 != r ? 255 * t / r : 0, 0 != i.s ? e.r == r ? i.h = (e.g - e.b) / t : e.g == r ? i.h = 2 + (e.b - e.r) / t : i.h = 4 + (e.r - e.g) / t : i.h = -1, r == o && (i.h = 0), i.h *= 60, i.h < 0 && (i.h += 360), i.s *= 100 / 255, i.b *= 100 / 255, i
		},
		y = function(e) {
			var e = e.indexOf("#") > -1 ? e.substring(1) : e;
			if(3 == e.length) {
				var i = e.split("");
				e = i[0] + i[0] + i[1] + i[1] + i[2] + i[2]
			}
			e = parseInt(e, 16);
			var o = {
				r: e >> 16,
				g: (65280 & e) >> 8,
				b: 255 & e
			};
			return k(o)
		},
		x = function(e) {
			var i = {},
				o = e.h,
				r = 255 * e.s / 100,
				t = 255 * e.b / 100;
			if(0 == r) i.r = i.g = i.b = t;
			else {
				var n = t,
					l = (255 - r) * t / 255,
					c = (n - l) * (o % 60) / 60;
				360 == o && (o = 0), o < 60 ? (i.r = n, i.b = l, i.g = l + c) : o < 120 ? (i.g = n, i.b = l, i.r = n - c) : o < 180 ? (i.g = n, i.r = l, i.b = l + c) : o < 240 ? (i.b = n, i.r = l, i.g = n - c) : o < 300 ? (i.b = n, i.g = l, i.r = l + c) : o < 360 ? (i.r = n, i.g = l, i.b = n - c) : (i.r = 0, i.g = 0, i.b = 0)
			}
			return {
				r: Math.round(i.r),
				g: Math.round(i.g),
				b: Math.round(i.b)
			}
		},
		C = function(e) {
			var o = x(e),
				r = [o.r.toString(16), o.g.toString(16), o.b.toString(16)];
			return i.each(r, function(e, i) {
				1 == i.length && (r[e] = "0" + i)
			}), r.join("")
		},
		P = function(e) {
			var i = /[0-9]{1,3}/g,
				o = e.match(i) || [];
			return {
				r: o[0],
				g: o[1],
				b: o[2]
			}
		},
		B = i(window),
		w = i(document),
		D = function(e) {
			var r = this;
			r.index = ++o.index, r.config = i.extend({}, r.config, o.config, e), r.render()
		};
	D.prototype.config = {
		color: "",
		size: null,
		alpha: !1,
		format: "hex",
		predefine: !1,
		colors: ["#009688", "#5FB878", "#1E9FFF", "#FF5722", "#FFB800", "#01AAED", "#999", "#c00", "#ff8c00", "#ffd700", "#90ee90", "#00ced1", "#1e90ff", "#c71585", "rgb(0, 186, 189)", "rgb(255, 120, 0)", "rgb(250, 212, 0)", "#393D49", "rgba(0,0,0,.5)", "rgba(255, 69, 0, 0.68)", "rgba(144, 240, 144, 0.5)", "rgba(31, 147, 255, 0.73)"]
	}, D.prototype.render = function() {
		var e = this,
			o = e.config,
			r = i(['<div class="layui-unselect layui-colorpicker">', "<span " + ("rgb" == o.format && o.alpha ? 'class="layui-colorpicker-trigger-bgcolor"' : "") + ">", '<span class="layui-colorpicker-trigger-span" ', 'lay-type="' + ("rgb" == o.format ? o.alpha ? "rgba" : "torgb" : "") + '" ', 'style="' + function() {
				var e = "";
				return o.color ? (e = o.color, (o.color.match(/[0-9]{1,3}/g) || []).length > 3 && (o.alpha && "rgb" == o.format || (e = "#" + C(k(P(o.color))))), "background: " + e) : e
			}() + '">', '<i class="layui-icon layui-colorpicker-trigger-i ' + (o.color ? a : s) + '"></i>', "</span>", "</span>", "</div>"].join("")),
			t = i(o.elem);
		o.size && r.addClass("layui-colorpicker-" + o.size), t.addClass("layui-inline").html(e.elemColorBox = r), e.color = e.elemColorBox.find("." + f)[0].style.background, e.events()
	}, D.prototype.renderPicker = function() {
		var e = this,
			o = e.config,
			r = e.elemColorBox[0],
			t = e.elemPicker = i(['<div id="layui-colorpicker' + e.index + '" data-index="' + e.index + '" class="layui-anim layui-anim-upbit layui-colorpicker-main">', '<div class="layui-colorpicker-main-wrapper">', '<div class="layui-colorpicker-basis">', '<div class="layui-colorpicker-basis-white"></div>', '<div class="layui-colorpicker-basis-black"></div>', '<div class="layui-colorpicker-basis-cursor"></div>', "</div>", '<div class="layui-colorpicker-side">', '<div class="layui-colorpicker-side-slider"></div>', "</div>", "</div>", '<div class="layui-colorpicker-main-alpha ' + (o.alpha ? n : "") + '">', '<div class="layui-colorpicker-alpha-bgcolor">', '<div class="layui-colorpicker-alpha-slider"></div>', "</div>", "</div>", function() {
				if(o.predefine) {
					var e = ['<div class="layui-colorpicker-main-pre">'];
					return layui.each(o.colors, function(i, o) {
						e.push(['<div class="layui-colorpicker-pre' + ((o.match(/[0-9]{1,3}/g) || []).length > 3 ? " layui-colorpicker-pre-isalpha" : "") + '">', '<div style="background:' + o + '"></div>', "</div>"].join(""))
					}), e.push("</div>"), e.join("")
				}
				return ""
			}(), '<div class="layui-colorpicker-main-input">', '<div class="layui-inline">', '<input type="text" class="layui-input">', "</div>", '<div class="layui-btn-container">', '<button class="layui-btn layui-btn-primary layui-btn-sm" colorpicker-events="clear">清空</button>', '<button class="layui-btn layui-btn-sm" colorpicker-events="confirm">确定</button>', "</div", "</div>", "</div>"].join(""));
		e.elemColorBox.find("." + f)[0];
		i(c)[0] && i(c).data("index") == e.index ? e.removePicker(D.thisElemInd) : (e.removePicker(D.thisElemInd), i("body").append(t)), D.thisElemInd = e.index, D.thisColor = r.style.background, e.position(), e.pickerEvents()
	}, D.prototype.removePicker = function(e) {
		var o = this;
		o.config;
		return i("#layui-colorpicker" + (e || o.index)).remove(), o
	}, D.prototype.position = function() {
		var e = this,
			i = e.config,
			o = e.bindElem || e.elemColorBox[0],
			r = e.elemPicker[0],
			t = o.getBoundingClientRect(),
			n = r.offsetWidth,
			l = r.offsetHeight,
			c = function(e) {
				return e = e ? "scrollLeft" : "scrollTop", document.body[e] | document.documentElement[e]
			},
			a = function(e) {
				return document.documentElement[e ? "clientWidth" : "clientHeight"]
			},
			s = 5,
			f = t.left,
			d = t.bottom;
		f -= (n - o.offsetWidth) / 2, d += s, f + n + s > a("width") ? f = a("width") - n - s : f < s && (f = s), d + l + s > a() && (d = t.top > l ? t.top - l : a() - l, d -= 2 * s), i.position && (r.style.position = i.position), r.style.left = f + ("fixed" === i.position ? 0 : c(1)) + "px", r.style.top = d + ("fixed" === i.position ? 0 : c()) + "px"
	}, D.prototype.val = function() {
		var e = this,
			i = (e.config, e.elemColorBox.find("." + f)),
			o = e.elemPicker.find("." + b),
			r = i[0],
			t = r.style.backgroundColor;
		if(t) {
			var n = k(P(t)),
				l = i.attr("lay-type");
			if(e.select(n.h, n.s, n.b), "torgb" === l && o.find("input").val(t), "rgba" === l) {
				var c = P(t);
				if(3 == (t.match(/[0-9]{1,3}/g) || []).length) o.find("input").val("rgba(" + c.r + ", " + c.g + ", " + c.b + ", 1)"), e.elemPicker.find("." + h).css("left", 280);
				else {
					o.find("input").val(t);
					var a = 280 * t.slice(t.lastIndexOf(",") + 1, t.length - 1);
					e.elemPicker.find("." + h).css("left", a)
				}
				e.elemPicker.find("." + v)[0].style.background = "linear-gradient(to right, rgba(" + c.r + ", " + c.g + ", " + c.b + ", 0), rgb(" + c.r + ", " + c.g + ", " + c.b + "))"
			}
		} else e.select(0, 100, 100), o.find("input").val(""), e.elemPicker.find("." + v)[0].style.background = "", e.elemPicker.find("." + h).css("left", 280)
	}, D.prototype.side = function() {
		var e = this,
			o = e.config,
			r = e.elemColorBox.find("." + f),
			t = r.attr("lay-type"),
			n = e.elemPicker.find("." + u),
			l = e.elemPicker.find("." + p),
			c = e.elemPicker.find("." + g),
			y = e.elemPicker.find("." + m),
			C = e.elemPicker.find("." + v),
			w = e.elemPicker.find("." + h),
			D = l[0].offsetTop / 180 * 360,
			E = 100 - (y[0].offsetTop + 3) / 180 * 100,
			H = (y[0].offsetLeft + 3) / 260 * 100,
			W = Math.round(w[0].offsetLeft / 280 * 100) / 100,
			j = e.elemColorBox.find("." + d),
			F = e.elemPicker.find(".layui-colorpicker-pre").children("div"),
			L = function(i, n, l, c) {
				e.select(i, n, l);
				var f = x({
					h: i,
					s: n,
					b: l
				});
				if(j.addClass(a).removeClass(s), r[0].style.background = "rgb(" + f.r + ", " + f.g + ", " + f.b + ")", "torgb" === t && e.elemPicker.find("." + b).find("input").val("rgb(" + f.r + ", " + f.g + ", " + f.b + ")"), "rgba" === t) {
					var d = 0;
					d = 280 * c, w.css("left", d), e.elemPicker.find("." + b).find("input").val("rgba(" + f.r + ", " + f.g + ", " + f.b + ", " + c + ")"), r[0].style.background = "rgba(" + f.r + ", " + f.g + ", " + f.b + ", " + c + ")", C[0].style.background = "linear-gradient(to right, rgba(" + f.r + ", " + f.g + ", " + f.b + ", 0), rgb(" + f.r + ", " + f.g + ", " + f.b + "))"
				}
				o.change && o.change(e.elemPicker.find("." + b).find("input").val())
			},
			M = i(['<div class="layui-auxiliar-moving" id="LAY-colorpicker-moving"></div'].join("")),
			Y = function(e) {
				i("#LAY-colorpicker-moving")[0] || i("body").append(M), M.on("mousemove", e), M.on("mouseup", function() {
					M.remove()
				}).on("mouseleave", function() {
					M.remove()
				})
			};
		l.on("mousedown", function(e) {
			var i = this.offsetTop,
				o = e.clientY,
				r = function(e) {
					var r = i + (e.clientY - o),
						t = n[0].offsetHeight;
					r < 0 && (r = 0), r > t && (r = t);
					var l = r / 180 * 360;
					D = l, L(l, H, E, W), e.preventDefault()
				};
			Y(r), e.preventDefault()
		}), n.on("click", function(e) {
			var o = e.clientY - i(this).offset().top;
			o < 0 && (o = 0), o > this.offsetHeight && (o = this.offsetHeight);
			var r = o / 180 * 360;
			D = r, L(r, H, E, W), e.preventDefault()
		}), y.on("mousedown", function(e) {
			var i = this.offsetTop,
				o = this.offsetLeft,
				r = e.clientY,
				t = e.clientX,
				n = function(e) {
					var n = i + (e.clientY - r),
						l = o + (e.clientX - t),
						a = c[0].offsetHeight - 3,
						s = c[0].offsetWidth - 3;
					n < -3 && (n = -3), n > a && (n = a), l < -3 && (l = -3), l > s && (l = s);
					var f = (l + 3) / 260 * 100,
						d = 100 - (n + 3) / 180 * 100;
					E = d, H = f, L(D, f, d, W), e.preventDefault()
				};
			layui.stope(e), Y(n), e.preventDefault()
		}), c.on("mousedown", function(e) {
			var o = e.clientY - i(this).offset().top - 3 + B.scrollTop(),
				r = e.clientX - i(this).offset().left - 3 + B.scrollLeft();
			o < -3 && (o = -3), o > this.offsetHeight - 3 && (o = this.offsetHeight - 3), r < -3 && (r = -3), r > this.offsetWidth - 3 && (r = this.offsetWidth - 3);
			var t = (r + 3) / 260 * 100,
				n = 100 - (o + 3) / 180 * 100;
			E = n, H = t, L(D, t, n, W), e.preventDefault(), y.trigger(e, "mousedown")
		}), w.on("mousedown", function(e) {
			var i = this.offsetLeft,
				o = e.clientX,
				r = function(e) {
					var r = i + (e.clientX - o),
						t = C[0].offsetWidth;
					r < 0 && (r = 0), r > t && (r = t);
					var n = Math.round(r / 280 * 100) / 100;
					W = n, L(D, H, E, n), e.preventDefault()
				};
			Y(r), e.preventDefault()
		}), C.on("click", function(e) {
			var o = e.clientX - i(this).offset().left;
			o < 0 && (o = 0), o > this.offsetWidth && (o = this.offsetWidth);
			var r = Math.round(o / 280 * 100) / 100;
			W = r, L(D, H, E, r), e.preventDefault()
		}), F.each(function() {
			i(this).on("click", function() {
				i(this).parent(".layui-colorpicker-pre").addClass("selected").siblings().removeClass("selected");
				var e, o = this.style.backgroundColor,
					r = k(P(o)),
					t = o.slice(o.lastIndexOf(",") + 1, o.length - 1);
				D = r.h, H = r.s, E = r.b, 3 == (o.match(/[0-9]{1,3}/g) || []).length && (t = 1), W = t, e = 280 * t, L(r.h, r.s, r.b, t)
			})
		})
	}, D.prototype.select = function(e, i, o, r) {
		var t = this,
			n = (t.config, C({
				h: e,
				s: 100,
				b: 100
			})),
			l = C({
				h: e,
				s: i,
				b: o
			}),
			c = e / 360 * 180,
			a = 180 - o / 100 * 180 - 3,
			s = i / 100 * 260 - 3;
		t.elemPicker.find("." + p).css("top", c), t.elemPicker.find("." + g)[0].style.background = "#" + n, t.elemPicker.find("." + m).css({
			top: a,
			left: s
		}), "change" !== r && t.elemPicker.find("." + b).find("input").val("#" + l)
	}, D.prototype.pickerEvents = function() {
		var e = this,
			o = e.config,
			r = e.elemColorBox.find("." + f),
			t = e.elemPicker.find("." + b + " input"),
			n = {
				clear: function(i) {
					r[0].style.background = "", e.elemColorBox.find("." + d).removeClass(a).addClass(s), e.color = "", o.done && o.done(""), e.removePicker()
				},
				confirm: function(i, n) {
					var l = t.val(),
						c = l,
						f = {};
					if(l.indexOf(",") > -1) {
						if(f = k(P(l)), e.select(f.h, f.s, f.b), r[0].style.background = c = "#" + C(f), (l.match(/[0-9]{1,3}/g) || []).length > 3 && "rgba" === r.attr("lay-type")) {
							var u = 280 * l.slice(l.lastIndexOf(",") + 1, l.length - 1);
							e.elemPicker.find("." + h).css("left", u), r[0].style.background = l, c = l
						}
					} else f = y(l), r[0].style.background = c = "#" + C(f), e.elemColorBox.find("." + d).removeClass(s).addClass(a);
					return "change" === n ? (e.select(f.h, f.s, f.b, n), void(o.change && o.change(c))) : (e.color = l, o.done && o.done(l), void e.removePicker())
				}
			};
		e.elemPicker.on("click", "*[colorpicker-events]", function() {
			var e = i(this),
				o = e.attr("colorpicker-events");
			n[o] && n[o].call(this, e)
		}), t.on("keyup", function(e) {
			var o = i(this);
			n.confirm.call(this, o, 13 === e.keyCode ? null : "change")
		})
	}, D.prototype.events = function() {
		var e = this,
			o = e.config,
			r = e.elemColorBox.find("." + f);
		e.elemColorBox.on("click", function() {
			e.renderPicker(), i(c)[0] && (e.val(), e.side())
		}), o.elem[0] && !e.elemColorBox[0].eventHandler && (w.on("click", function(o) {
			if(!i(o.target).hasClass(l) && !i(o.target).parents("." + l)[0] && !i(o.target).hasClass(c.replace(/\./g, "")) && !i(o.target).parents(c)[0] && e.elemPicker) {
				if(e.color) {
					var t = k(P(e.color));
					e.select(t.h, t.s, t.b)
				} else e.elemColorBox.find("." + d).removeClass(a).addClass(s);
				r[0].style.background = e.color || "", e.removePicker()
			}
		}), B.on("resize", function() {
			return !(!e.elemPicker || !i(c)[0]) && void e.position()
		}), e.elemColorBox[0].eventHandler = !0)
	}, o.render = function(e) {
		var i = new D(e);
		return r.call(i)
	}, e(t, o)
});