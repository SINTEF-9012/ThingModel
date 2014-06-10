![ThingModel](https://raw.github.com/SINTEF-9012/ThingModel/master/Documentation/Logo.png)
==========

ThingModel synchronizes data and models in realtime over the network for multiple devices. Mainly iPads, PC and a PixelSense table.

ThingModel is a new wheel, but a simple, fast and light one.

ThingModel does not need compilation, it does not include a DSL either and everything is done during runtime. You can build the model on the fly and the connected applications can learn it during their executions.

If you are looking for innovative products about models, you should also take a look at [ThingML](http://thingml.org/) or [Kevoree](http://kevoree.org/kmf/).

## The model

![Model](https://raw2.github.com/SINTEF-9012/ThingModel/master/Documentation/ThingModel.png)

## Light network synchronization

 * Clients and servers over WebSockets
 * Light serialization with Protocol Buffers
 * Diff algorithm, only send changes
 * String cache for smaller messages

## Multi-plateform

 * __C# .Net__
  * For the PixelSense table
  * And because C# is cool
 * __JavaScript__ (and TypeScript)
  * Recent HTML5 browsers (need WebSocket support)
  * Can work in Node.JS
 * __Java__
  * Mainly for Minecraft
  * Or a HLA bridge for VBS

## A collection of tools

### A broadcast server

![Screenshot server](https://raw2.github.com/SINTEF-9012/ThingModel/master/Documentation/Screenshot-Server.png)

If this server doesn't suit your needs, you can easely develop your own ThingModel server.

### A time machine

![Screenshot timemachine](https://raw2.github.com/SINTEF-9012/ThingModel/master/Documentation/Screenshot-TimeMachine.png)

The time machine saves the model in realtime (1 hertz as maximum frequency by default). Thanks to protocol buffers, gzip and sqlite, it's very light.

And you can go back in time just with a HTTP request.

## Bugs

ThingModel is tested and used almost every days and it works, but *it's not stable*.

Please follow [the checklist](https://github.com/SINTEF-9012/ThingModel/wiki/Checklist) if you are experiencing some weird problems.

## Installation

__Nuget__: ```Install-Package ThingModel -Pre```

__Bower__: ```bower install ThingModel```

__Maven__: *need motivation*

Otherwise: [__Download__](https://github.com/SINTEF-9012/ThingModel/archive/master.zip)

## Example (in CSharp)

```csharp

// Declare the rabbit type
var typeRabbit = BuildANewThingType.Named("rabbit")
				.WhichIs("Just a rabbit")
				.ContainingA.String("name")
				.AndA.LocationLatLng()
				.AndA.NotRequired.Double("speed")
				.AndAn.Int("nbChildren", "Number of children");
				
// Create a rabbit instance
var rabbit = BuildANewThing.As(typeRabbit)
				.IdentifiedBy("ab548")
				.ContainingA.String("name", "Roger")
				.AndA.LocationLatLng(new Location.LatLng(60,10))
				.AndAn.Int("nbChildren", 12);

// The warehouse stores the objects
var warehouse = new Warehouse();

// Create a websocket client
var client = new Client("Emitter", "ws://localhost:1234", warehouse);

// Register the rabbit
warehouse.RegisterThing(rabbit);

// Send the changes to the server
client.Send();
```

```csharp
var warehouse = new Warehouse();
var client = new Client("Receiver", "ws://localhost:1234", warehouse);

warehouse.Events.OnNew += (sender, args) => {
    Console.WriteLine(args.Thing.String("name"));
};
```
