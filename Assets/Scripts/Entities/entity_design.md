# Entity Design #1

This document is a sketch of how entities will work.

## The Entity
At the top level, we have the Entity. The entity is a composite object containing entity parts. 

The entity itself supports some behaviors:

* Add part
* Remove part
* Teleport
* Get location
* GameUpdate (the core game loop)

Along with those behaviors of PersistableObject.

The entity also supports a system of slots. Upon adding a part, the entity asks the part to mount itself into any relevant slots. We will implement slots via typed fields.

## EntityParts

EntityParts will be small pieces of behavior that we may apply to entities to define gameplay logic.

EntityParts will be non-GameObjects that the game will load via code. A successful solution will allow us to fulfill a few actions:

* Instantiate an entity with a given set of parts.
* Modify the parts of an entity during gameplay.
* Save the entity's part state.
* Reload the entity's previous part state.

We may achieve these goals in two ways:

### Strategy #1
Entities are unique prefabs with a single game script that builds them out on load.

See Reloading Entities below. We will need a mechanism for disabling the build-out when reloading previously serialized entities.

### Strategy #2
Entities are configured by the factory (and subordinate code) based on the requested type.

The loader can request a bare version of the entity.

## The EntityPart factory

## Reloading Entities
We should expect a bare entity that the loader can use to read entity parts from a file. This entity will behave the same as a newly produced entity from the factory.