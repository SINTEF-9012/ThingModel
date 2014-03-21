![ThingModel](https://raw.github.com/SINTEF-9012/ThingModel/master/Documentation/Logo.png)
==========

![Model](https://raw2.github.com/SINTEF-9012/ThingModel/master/Documentation/ThingModel.png)

### Simple

ThingModel is simple and like a new wheel.

It shares data and models over the network for multiple devices. Mainly iPads, PC and a PixelSense table.

If you are looking for a innovative product about models, you should take a look at [ThingML](http://thingml.org/) or [Kevoree](http://kevoree.org/kmf/).


### Light network synchronization

 * Clients and servers over WebSockets
 * Light serialization with Protocol Buffers
 * Diff algorithm, for very lights synchronizations

### Bugs

ThingModel is used almost every days. It's a test driven development, but *it's not stable*.

### Multi-plateform

 * C# .Net
  * For the PixelSense table
  * And because C# is cool
 * JavaScript (and TypeScript)
  * Recent HTML5 browsers (need WebSocket support)
  * Can work in Node.JS
 * Java
  * Mainly for Minecraft
  * Or a HLA bridge

### Installation

__Nuget__: ```Install-Package ThingModel```

__Bower__: ```bower install ThingModel```

__Maven__: *need motivation*

Otherwise: [__Download__](https://github.com/SINTEF-9012/ThingModel/archive/master.zip)
