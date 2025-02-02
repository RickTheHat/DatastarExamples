/**
 * Bundled by jsDelivr using Rollup v2.79.2 and Terser v5.37.0.
 * Original file: /npm/@starfederation/datastar@1.0.0-beta.3/dist/index.js
 *
 * Do NOT use SRI with dynamically generated files! More information: https://www.jsdelivr.com/using-sri-with-dynamic-files
 */
const t=/🖕JS_DS🚀/.source,e=t.slice(0,5),s=t.slice(4),n="datastar";var i,o;!function(t){t[t.Attribute=1]="Attribute",t[t.Watcher=2]="Watcher",t[t.Action=3]="Action"}(i||(i={})),function(t){t[t.Allowed=0]="Allowed",t[t.Must=1]="Must",t[t.Denied=2]="Denied",t[t.Exclusive=3]="Exclusive"}(o||(o={}));const r=`${n}-signals`,a={type:i.Attribute,name:"computed",keyReq:o.Must,valReq:o.Must,onLoad:({key:t,signals:e,genRX:s})=>{const n=s();e.setComputed(t,n)}},l=t=>t[0].toLowerCase()+t.slice(1),c=t=>t.replace(/[A-Z]+(?![a-z])|[A-Z]/g,((t,e)=>(e?"-":"")+t.toLowerCase())),u=t=>c(t).replace(/-./g,(t=>t[1].toUpperCase())),h={type:i.Attribute,name:"signals",removeOnLoad:()=>!0,onLoad:t=>{const{key:e,value:s,genRX:n,signals:i,mods:o}=t,r=o.has("ifmissing");if(""===e||r){const e=(a=t.value,new Function(`return Object.assign({}, ${a})`)());t.value=JSON.stringify(e);const s=n()();i.merge(s,r)}else{const t=""===s?s:n()();i.setValue(e,t)}var a}},f={type:i.Attribute,name:"star",keyReq:o.Denied,valReq:o.Denied,onLoad:()=>{alert("YOU ARE PROBABLY OVERCOMPLICATING IT")}};class d{#t=0;#e;constructor(t=n){this.#e=t}with(t){if("string"==typeof t)for(const e of t.split(""))this.with(e.charCodeAt(0));else this.#t=(this.#t<<5)-this.#t+t;return this}get value(){return this.#e+Math.abs(this.#t).toString(36)}}function _(t,e,s={}){const i=new Error;e=e[0].toUpperCase()+e.slice(1),i.name=`${n} ${t} error`;const o=c(e).replaceAll("-","_"),r=new URLSearchParams({metadata:JSON.stringify(s)}).toString(),a=JSON.stringify(s,null,2);return i.message=`${e}\nMore info: https://data-star.dev/errors/${t}/${o}?${r}\nContext: ${a}`,i}function g(t,e,s={}){return _("internal",e,Object.assign({from:t},s))}function p(t,e,s={}){const n={plugin:{name:e.plugin.name,type:i[e.plugin.type]}};return _("init",t,Object.assign(n,s))}function v(t,e,s={}){const n={plugin:{name:e.plugin.name,type:i[e.plugin.type]},element:{id:e.el.id,tag:e.el.tagName},expression:{rawKey:e.rawKey,key:e.key,value:e.value,validSignals:e.signals.paths(),fnContent:e.fnContent}};return _("runtime",t,Object.assign(n,s))}const y="preact-signals",m=Symbol.for("preact-signals"),b=32;function w(){if(O>1)return void O--;let t,e=!1;for(;void 0!==x;){let s=x;for(x=void 0,$++;void 0!==s;){const n=s._nextBatchedEffect;if(s._nextBatchedEffect=void 0,s._flags&=-3,!(8&s._flags)&&N(s))try{s._callback()}catch(s){e||(t=s,e=!0)}s=n}}if($=0,O--,e)throw g(y,"BatchError, error",{error:t})}let S,x,O=0,$=0,A=0;function E(t){if(void 0===S)return;let e=t._node;return void 0===e||e._target!==S?(e={_version:0,_source:t,_prevSource:S._sources,_nextSource:void 0,_target:S,_prevTarget:void 0,_nextTarget:void 0,_rollbackNode:e},void 0!==S._sources&&(S._sources._nextSource=e),S._sources=e,t._node=e,S._flags&b&&t._subscribe(e),e):-1===e._version?(e._version=0,void 0!==e._nextSource&&(e._nextSource._prevSource=e._prevSource,void 0!==e._prevSource&&(e._prevSource._nextSource=e._nextSource),e._prevSource=S._sources,e._nextSource=void 0,S._sources._nextSource=e,S._sources=e),e):void 0}function k(t){this._value=t,this._version=0,this._node=void 0,this._targets=void 0}function N(t){for(let e=t._sources;void 0!==e;e=e._nextSource)if(e._source._version!==e._version||!e._source._refresh()||e._source._version!==e._version)return!0;return!1}function C(t){for(let e=t._sources;void 0!==e;e=e._nextSource){const s=e._source._node;if(void 0!==s&&(e._rollbackNode=s),e._source._node=e,e._version=-1,void 0===e._nextSource){t._sources=e;break}}}function R(t){let e,s=t._sources;for(;void 0!==s;){const t=s._prevSource;-1===s._version?(s._source._unsubscribe(s),void 0!==t&&(t._nextSource=s._nextSource),void 0!==s._nextSource&&(s._nextSource._prevSource=t)):e=s,s._source._node=s._rollbackNode,void 0!==s._rollbackNode&&(s._rollbackNode=void 0),s=t}t._sources=e}function M(t){k.call(this,void 0),this._fn=t,this._sources=void 0,this._globalVersion=A-1,this._flags=4}function T(t){const e=t._cleanup;if(t._cleanup=void 0,"function"==typeof e){O++;const s=S;S=void 0;try{e()}catch(e){throw t._flags&=-2,t._flags|=8,j(t),g(y,"CleanupEffectError",{error:e})}finally{S=s,w()}}}function j(t){for(let e=t._sources;void 0!==e;e=e._nextSource)e._source._unsubscribe(e);t._fn=void 0,t._sources=void 0,T(t)}function D(t){if(S!==this)throw g(y,"EndEffectError");R(this),S=t,this._flags&=-2,8&this._flags&&j(this),w()}function L(t){this._fn=t,this._cleanup=void 0,this._sources=void 0,this._nextBatchedEffect=void 0,this._flags=b}function P(t){const e=new L(t);try{e._callback()}catch(t){throw e._dispose(),t}return e._dispose.bind(e)}k.prototype.brand=m,k.prototype._refresh=()=>!0,k.prototype._subscribe=function(t){this._targets!==t&&void 0===t._prevTarget&&(t._nextTarget=this._targets,void 0!==this._targets&&(this._targets._prevTarget=t),this._targets=t)},k.prototype._unsubscribe=function(t){if(void 0!==this._targets){const e=t._prevTarget,s=t._nextTarget;void 0!==e&&(e._nextTarget=s,t._prevTarget=void 0),void 0!==s&&(s._prevTarget=e,t._nextTarget=void 0),t===this._targets&&(this._targets=s)}},k.prototype.subscribe=function(t){return P((()=>{const e=this.value,s=S;S=void 0;try{t(e)}finally{S=s}}))},k.prototype.valueOf=function(){return this.value},k.prototype.toString=function(){return`${this.value}`},k.prototype.toJSON=function(){return this.value},k.prototype.peek=function(){const t=S;S=void 0;try{return this.value}finally{S=t}},Object.defineProperty(k.prototype,"value",{get(){const t=E(this);return void 0!==t&&(t._version=this._version),this._value},set(t){if(t!==this._value){if($>100)throw g(y,"SignalCycleDetected");this._value=t,this._version++,A++,O++;try{for(let t=this._targets;void 0!==t;t=t._nextTarget)t._target._notify()}finally{w()}}}}),M.prototype=new k,M.prototype._refresh=function(){if(this._flags&=-3,1&this._flags)return!1;if((36&this._flags)===b)return!0;if(this._flags&=-5,this._globalVersion===A)return!0;if(this._globalVersion=A,this._flags|=1,this._version>0&&!N(this))return this._flags&=-2,!0;const t=S;try{C(this),S=this;const t=this._fn();(16&this._flags||this._value!==t||0===this._version)&&(this._value=t,this._flags&=-17,this._version++)}catch(t){this._value=t,this._flags|=16,this._version++}return S=t,R(this),this._flags&=-2,!0},M.prototype._subscribe=function(t){if(void 0===this._targets){this._flags|=36;for(let t=this._sources;void 0!==t;t=t._nextSource)t._source._subscribe(t)}k.prototype._subscribe.call(this,t)},M.prototype._unsubscribe=function(t){if(void 0!==this._targets&&(k.prototype._unsubscribe.call(this,t),void 0===this._targets)){this._flags&=-33;for(let t=this._sources;void 0!==t;t=t._nextSource)t._source._unsubscribe(t)}},M.prototype._notify=function(){if(!(2&this._flags)){this._flags|=6;for(let t=this._targets;void 0!==t;t=t._nextTarget)t._target._notify()}},Object.defineProperty(M.prototype,"value",{get(){if(1&this._flags)throw g(y,"SignalCycleDetected");const t=E(this);if(this._refresh(),void 0!==t&&(t._version=this._version),16&this._flags)throw g(y,"GetComputedError",{value:this._value});return this._value}}),L.prototype._callback=function(){const t=this._start();try{if(8&this._flags)return;if(void 0===this._fn)return;const t=this._fn();"function"==typeof t&&(this._cleanup=t)}finally{t()}},L.prototype._start=function(){if(1&this._flags)throw g(y,"SignalCycleDetected");this._flags|=1,this._flags&=-9,T(this),C(this),O++;const t=S;return S=this,D.bind(this,t)},L.prototype._notify=function(){2&this._flags||(this._flags|=2,this._nextBatchedEffect=x,x=this)},L.prototype._dispose=function(){this._flags|=8,1&this._flags||j(this)};const V="namespacedSignals",I=t=>{document.dispatchEvent(new CustomEvent(r,{detail:Object.assign({added:[],removed:[],updated:[]},t)}))};function q(t,e=!1){const s={};for(const n in t)if(Object.hasOwn(t,n)){if(e&&n.startsWith("_"))continue;const i=t[n];s[n]=i instanceof k?i.value:q(i)}return s}function J(t,e,s=!1){const n={added:[],removed:[],updated:[]};for(const i in e)if(Object.hasOwn(e,i)){if(i.match(/\_\_+/))throw g(V,"InvalidSignalKey",{key:i});const o=e[i];if(o instanceof Object&&!Array.isArray(o)){t[i]||(t[i]={});const e=J(t[i],o,s);n.added.push(...e.added.map((t=>`${i}.${t}`))),n.removed.push(...e.removed.map((t=>`${i}.${t}`))),n.updated.push(...e.updated.map((t=>`${i}.${t}`)))}else{if(Object.hasOwn(t,i)){if(s)continue;const e=t[i];if(e instanceof k){const t=e.value;e.value=o,t!==o&&n.updated.push(i);continue}}t[i]=new k(o),n.added.push(i)}}return n}function W(t,e){for(const s in t)if(Object.hasOwn(t,s)){const n=t[s];n instanceof k?e(s,n):W(n,((t,n)=>{e(`${s}.${t}`,n)}))}}class K{#s={};exists(t){return!!this.signal(t)}signal(t){const e=t.split(".");let s=this.#s;for(let t=0;t<e.length-1;t++){const n=e[t];if(!s[n])return null;s=s[n]}const n=s[e[e.length-1]];if(!n)throw g(V,"SignalNotFound",{path:t});return n}setSignal(t,e){const s=t.split(".");let n=this.#s;for(let t=0;t<s.length-1;t++){const e=s[t];n[e]||(n[e]={}),n=n[e]}n[s[s.length-1]]=e}setComputed(t,e){const s=function(t){return new M(t)}((()=>e()));this.setSignal(t,s)}value(t){const e=this.signal(t);return e?.value}setValue(t,e){const s=this.upsertIfMissing(t,e),n=s.value;s.value=e,n!==e&&I({updated:[t]})}upsertIfMissing(t,e){const s=t.split(".");let n=this.#s;for(let t=0;t<s.length-1;t++){const e=s[t];n[e]||(n[e]={}),n=n[e]}const i=s[s.length-1],o=n[i];if(o instanceof k)return o;const r=new k(e);return n[i]=r,I({added:[t]}),r}remove(...t){if(!t.length)return void(this.#s={});const e=Array();for(const s of t){const t=s.split(".");let n=this.#s;for(let e=0;e<t.length-1;e++){const s=t[e];if(!n[s])return;n=n[s]}delete n[t[t.length-1]],e.push(s)}I({removed:e})}merge(t,e=!1){const s=J(this.#s,t,e);(s.added.length||s.removed.length||s.updated.length)&&I(s)}subset(...t){return function(t,...e){const s={};for(const n of e){const e=n.split(".");let i=t,o=s;for(let t=0;t<e.length-1;t++){const s=e[t];if(!i[s])return{};o[s]||(o[s]={}),i=i[s],o=o[s]}const r=e[e.length-1];o[r]=i[r]}return s}(this.values(),...t)}walk(t){W(this.#s,t)}paths(){const t=new Array;return this.walk((e=>t.push(e))),t}values(t=!1){return q(this.#s,t)}JSON(t=!0,e=!1){const s=this.values(e);return t?JSON.stringify(s,null,2):JSON.stringify(s)}toString(){return this.JSON()}}const B=(t,s)=>`${t}${e}${s}`;const G=new class{#s;#n;#i;#o;#r;constructor(){this.aliasPrefix="",this.#s=new K,this.#n=[],this.#i={},this.#o=[],this.#r=new Map,this.#a=function(t,e,s=!1,n=!0){let i=-1;const o=()=>i&&clearTimeout(i);return(...r)=>{o(),s&&!i&&t(...r),i=setTimeout((()=>{n&&t(...r),o()}),e)}}((()=>{this.#l(document.body)}),1);const t="data-";new MutationObserver((e=>{for(const{target:s,type:n,attributeName:i,oldValue:o,addedNodes:r,removedNodes:a}of e)switch(n){case"childList":for(const t of a){const e=t,s=this.#r.get(e);if(s){for(const[t,e]of s)e();this.#r.delete(e)}}for(const t of r){const e=t;this.#l(e)}break;case"attributes":{const e=t+(this.aliasPrefix?`${this.aliasPrefix}-`:"");if(!i?.startsWith(e))break;const n=s,r=u(i.slice(5));if(null!==o&&n.dataset[r]!==o){const t=this.#r.get(n);if(t){const e=B(r,o),s=t.get(e);s&&(s(),t.delete(e))}}r in n.dataset&&this.#c(n,r)}}})).observe(document.body,{attributes:!0,attributeOldValue:!0,childList:!0,subtree:!0})}get signals(){return this.#s}load(...t){for(const e of t){const t=this,s={get signals(){return t.#s},effect:t=>P(t),actions:this.#i,plugin:e};let n;switch(e.type){case i.Watcher:{const t=e;this.#o.push(t),n=t.onGlobalInit;break}case i.Action:this.#i[e.name]=e;break;case i.Attribute:{const t=e;this.#n.push(t),n=t.onGlobalInit;break}default:throw p("InvalidPluginType",s)}n&&n(s)}this.#n.sort(((t,e)=>{const s=e.name.length-t.name.length;return 0!==s?s:t.name.localeCompare(e.name)})),this.#a()}#a;#l(t){this.#u(t,(t=>{const e=this.#r.get(t);if(e){for(const[,t]of e)t();this.#r.delete(t)}for(const e of Object.keys(t.dataset))this.#c(t,e)}))}#c(t,e){const s=l(e.slice(this.aliasPrefix.length)),n=this.#n.find((t=>s.startsWith(t.name)));if(!n)return;t.id.length||(t.id=function(t){if(t.id)return t.id;const e=new d;let s=t;for(;s.parentNode;){if(s.id){e.with(s.id);break}if(s===s.ownerDocument.documentElement)e.with(s.tagName);else for(let s=1,n=t;n.previousElementSibling;n=n.previousElementSibling,s++)e.with(s);s=s.parentNode}return e.value}(t));let[i,...r]=s.slice(n.name.length).split(/\_\_+/);const a=i.length>0;a&&(i=i.startsWith("-")?i.slice(1):l(i));const c=t.dataset[e]||"",h=c.length>0,f=this,_={get signals(){return f.#s},effect:t=>P(t),actions:this.#i,genRX:()=>this.#h(_,...n.argNames||[]),plugin:n,el:t,rawKey:s,key:i,value:c,mods:new Map},g=n.keyReq||o.Allowed;if(a){if(g===o.Denied)throw v(`${n.name}KeyNotAllowed`,_)}else if(g===o.Must)throw v(`${n.name}KeyRequired`,_);const p=n.valReq||o.Allowed;if(h){if(p===o.Denied)throw v(`${n.name}ValueNotAllowed`,_)}else if(p===o.Must)throw v(`${n.name}ValueRequired`,_);if(g===o.Exclusive||p===o.Exclusive){if(a&&h)throw v(`${n.name}KeyAndValueProvided`,_);if(!a&&!h)throw v(`${n.name}KeyOrValueRequired`,_)}for(const t of r){const[e,...s]=t.split(".");_.mods.set(u(e),new Set(s.map((t=>t.toLowerCase()))))}const y=n.onLoad(_);if(y){let s=this.#r.get(t);s||(s=new Map,this.#r.set(t,s)),s.set(B(e,c),y)}const m=n.removeOnLoad;m&&!0===m(s)&&delete t.dataset[e]}#h(t,...n){let i="";const o=t.value.trim().match(/(\/(\\\/|[^\/])*\/|"(\\"|[^\"])*"|'(\\'|[^'])*'|`(\\`|[^`])*`|[^;])+/gm);if(o){const t=o.length-1,e=o[t].trim();e.startsWith("return")||(o[t]=`return (${e});`),i=o.join(";\n")}const r=new Map,a=new RegExp(`(?:${e})(.*?)(?:${s})`,"gm");for(const t of i.matchAll(a)){const n=t[1],o=new d("dsEscaped").with(n).value;r.set(o,n),i=i.replace(e+n+s,o)}const l=i.matchAll(/@(\w*)\(/gm),c=new Set;for(const t of l)c.add(t[1]);const u=new RegExp(`@(${Object.keys(this.#i).join("|")})\\(`,"gm");i=i.replaceAll(u,"ctx.actions.$1.fn(ctx,");const h=t.signals.paths();if(h.length){const t=new RegExp(`\\$(${h.join("|")})(\\W|$)`,"gm");i=i.replaceAll(t,"ctx.signals.signal('$1').value$2")}for(const[t,e]of r)i=i.replace(t,e);const f=`return (()=> {\n${i}\n})()`;t.fnContent=f;try{const e=new Function("ctx",...n,f);return(...s)=>{try{return e(t,...s)}catch(e){throw v("ExecuteExpression",t,{error:e.message})}}}catch(e){throw v("GenerateExpression",t,{error:e.message})}}#u(t,e){if(!t||!(t instanceof HTMLElement||t instanceof SVGElement))return null;const s=t.dataset;if("starIgnore"in s)return null;"starIgnore__self"in s||e(t);let n=t.firstElementChild;for(;n;)this.#u(n,e),n=n.nextElementSibling}};G.load(f,h,a);const X=G;export{X as Datastar};export default null;
//# sourceMappingURL=/sm/d8e765589c9ad398d78dbbad5ab1dc0f55e07e5c93fdb5e759773cfe16458488.map