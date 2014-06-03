<?xml version="1.0" encoding="utf-8"?>
<html xmlns:parse="http://www.rosivm.org/2014/parse/">
  <style>
        .Visibility, .class, .module, .interface, .function, .method {color:#04A; font-weight:bold;}
        .Name {color:#A0A; font-weight:bold;}
        .Type {color:#00A; font-weight:bold;}
        .var, .turns-in {color:#AAA; font-weight:bold;}
        {color:#AAA; font-weight:bold;}
        .ConstantValue {color:#A00; font-weight:bold;}
        .if, .else {color:#600; font-weight:bold;}
      </style>
  <body>
    <code>
<span class="module">module</span> <span class="Name">structures</span> <span class="l-brac">{</span><br />
  <span class="Visibility">public</span> <span class="interface">interface</span> <span class="Name">Cloneable</span> <span class="l-brac">{</span><br />
    <span class="method">method</span> <span class="Name">clone</span><span class="l-paren">(</span><span class="r-paren">)</span> <span class="turns-in">=&gt;</span> <span class="Identifier">object</span><span class="semicolon">;</span><br />
  <span class="r-brac">}</span><br />
  <span class="Visibility">public</span> <span class="interface">interface</span> <span class="Name">ListView</span> <span class="l-brac">{</span><br />
    <span class="method">method</span> <span class="Name">get</span><span class="l-paren">(</span><span class="r-paren">)</span> <span class="turns-in">=&gt;</span> <span class="Identifier">T</span><span class="semicolon">;</span><br />
  <span class="r-brac">}</span><br />
  <span class="Visibility">public</span> <span class="interface">interface</span> <span class="Name">List</span> <span class="l-brac">{</span><br />
    <span class="method">method</span> <span class="Name">get</span><span class="l-paren">(</span><span class="r-paren">)</span> <span class="turns-in">=&gt;</span> <span class="Identifier">T</span><span class="semicolon">;</span><br />
    <span class="method">method</span> <span class="Name">set</span><span class="l-paren">(</span><span class="r-paren">)</span><span class="semicolon">;</span><br />
  <span class="r-brac">}</span><br />
  <span class="Visibility">public</span> <span class="interface">interface</span> <span class="Name">Map</span> <span class="l-brac">{</span><br />
    <span class="method">method</span> <span class="Name">get</span><span class="l-paren">(</span><span class="r-paren">)</span> <span class="turns-in">=&gt;</span> <span class="Identifier">Value</span><span class="semicolon">;</span><br />
  <span class="r-brac">}</span><br />
<span class="r-brac">}</span><br />
<span class="module">module</span> <span class="Name">math</span> <span class="l-brac">{</span><br />
  <span class="Visibility">public</span> <span class="function">function</span> <span class="Name">min</span><span class="l-paren">(</span><span class="r-paren">)</span> <span class="turns-in">=&gt;</span> <span class="Identifier">float64</span> <span class="l-brac">{</span><br />
    <span class="Name">obj1</span><span class="l-paren">(</span><span class="Name">x</span><span class="comma">,</span> <span class="ConstantValue">12</span><span class="multiply">*</span><span class="Name">y</span><span class="r-paren">)</span><span class="semicolon">;</span><br />
    <span class="Name">obj2</span><span class="l-paren">(</span><span class="r-paren">)</span><span class="semicolon">;</span><br />
    <span class="Name">obj3</span><span class="l-paren">(</span><span class="ConstantValue">12</span><span class="plus">+</span><span class="Name">y</span><span class="multiply">*</span><span class="Name">x</span><span class="r-paren">)</span><span class="semicolon">;</span><br />
  <span class="r-brac">}</span><br />
  <span class="Visibility">public</span> <span class="function">function</span> <span class="Name">max</span><span class="l-paren">(</span><span class="r-paren">)</span> <span class="turns-in">=&gt;</span> <span class="Identifier">float64</span> <span class="l-brac">{</span><br />
  <span class="r-brac">}</span><br />
  <span class="Visibility">public</span> <span class="function">function</span> <span class="Name">sin</span><span class="l-paren">(</span><span class="r-paren">)</span> <span class="turns-in">=&gt;</span> <span class="Identifier">float64</span> <span class="l-brac">{</span><br />
  <span class="r-brac">}</span><br />
  <span class="Visibility">public</span> <span class="function">function</span> <span class="Name">cos</span><span class="l-paren">(</span><span class="r-paren">)</span> <span class="turns-in">=&gt;</span> <span class="Identifier">float64</span> <span class="l-brac">{</span><br />
  <span class="r-brac">}</span><br />
  <span class="Visibility">public</span> <span class="function">function</span> <span class="Name">abs</span><span class="l-paren">(</span><span class="r-paren">)</span> <span class="turns-in">=&gt;</span> <span class="Identifier">float64</span> <span class="l-brac">{</span><br />
  <span class="r-brac">}</span><br />
<span class="r-brac">}</span><br />
<span class="module">module</span> <span class="Name">geometry</span> <span class="l-brac">{</span><br />
  <span class="Visibility">public</span> <span class="interface">interface</span> <span class="Name">Equatible</span> <span class="l-brac">{</span><br />
    <span class="method">method</span> <span class="Name">equals</span><span class="l-paren">(</span><span class="r-paren">)</span> <span class="turns-in">=&gt;</span> <span class="Identifier">bool</span><span class="semicolon">;</span><br />
  <span class="r-brac">}</span><br />
  <span class="Visibility">public</span> <span class="class">class</span> <span class="Name">Point</span> <span class="l-brac">{</span><br />
    <span class="Visibility">public</span> <span class="method">method</span> <span class="Name">add</span><span class="l-paren">(</span><span class="r-paren">)</span> <span class="l-brac">{</span><br />
      <span class="Name">this</span><span class="dot">.</span><span class="Member">x</span><span class="assign">=</span><span class="ConstantValue">12</span><span class="plus">+</span><span class="ConstantValue">15</span><span class="semicolon">;</span><br />
      <span class="Name">this</span><span class="dot">.</span><span class="Member">y</span><span class="assign">=</span><span class="ConstantValue">13</span><span class="multiply">*</span><span class="ConstantValue">16</span><span class="increment">++</span><span class="semicolon">;</span><br />
      <span class="if">if</span><span class="l-paren">(</span><span class="Name">x</span><span class="greater-than">&gt;</span><span class="ConstantValue">15</span><span class="r-paren">)</span><span class="Name">large</span><span class="l-paren">(</span><span class="r-paren">)</span><span class="semicolon">;</span><span class="else">else</span><span class="Name">small</span><span class="l-paren">(</span><span class="r-paren">)</span><span class="semicolon">;</span><br />
      <span class="if">if</span><span class="l-paren">(</span><span class="not">!</span><span class="Name">x</span><span class="r-paren">)</span><span class="l-brac">{</span><br />
        <span class="if">if</span><span class="l-paren">(</span><span class="not">!</span><span class="Name">y</span><span class="r-paren">)</span><span class="l-brac">{</span><br />
          <span class="Name">large</span><span class="l-paren">(</span><span class="r-paren">)</span><span class="semicolon">;</span><br />
        <span class="r-brac">}</span><span class="else">else</span><span class="l-brac">{</span><br />
          <span class="Name">small</span><span class="l-paren">(</span><span class="r-paren">)</span><span class="semicolon">;</span><br />
        <span class="r-brac">}</span><br />
      <span class="r-brac">}</span><span class="else">else</span><span class="l-brac">{</span><br />
        <span class="Name">verySmall</span><span class="l-paren">(</span><span class="r-paren">)</span><span class="semicolon">;</span><br />
      <span class="r-brac">}</span><br />
    <span class="r-brac">}</span><br />
  <span class="r-brac">}</span><br />
  <span class="Visibility">public</span> <span class="class">class</span> <span class="Name">Point3d</span> <span class="l-brac">{</span><br />
  <span class="r-brac">}</span><br />
<span class="r-brac">}</span><br />

</code>
  </body>
</html>