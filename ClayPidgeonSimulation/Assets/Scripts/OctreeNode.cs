using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctreeNode {

    public static bool collision;
    public static float collisionCount = 0f;
    public BoundingBox BBox;
    
    private OctreeNode _Parent;
    private List<OctreeNode> Children;
    private List<GameObject> Objects; 

    public OctreeNode Parent {
        set {
            Debug.Assert(_Parent == null, "Node already has a parent!");
            _Parent = value;
        }
        get { return _Parent; }
    }
    
    // Create a list of all  game objects that have been detected
    public List<GameObject> GetObjects() { 
        return Objects;
    }

    // Constructor 
    public OctreeNode(Vec3 origin, Vec3 extents) {
        // create the bounding box for the node
        BBox = new BoundingBox(origin, extents);
        // Lazy initialisation, we will only allocate these as they are required
        _Parent = null;
        Children = null;
        Objects = null;
    }

    // creating subdivison nodes
    public void MakeChildren() {
        Debug.Assert(Children == null, "Children already present on this OctreeNode");
        Children = new List<OctreeNode>();
        // Create the 8 child objects for this Octree Node
        // Calculate the bounds of the child objects (simply multiply by 0.5 in each dimension)
        Vec3 extents = BBox.Extents * 0.5f;
        //Origin is in the center of the bouding box offset is half new bounds in each dimension
        Vec3 origin = BBox.position;
        
        // Drawing the Octree 
        Children.Add(new OctreeNode(new Vec3(origin.x - extents.x, origin.y - extents.y, origin.z - extents.z), extents));
        Children.Add(new OctreeNode(new Vec3(origin.x - extents.x, origin.y - extents.y, origin.z + extents.z), extents));
        Children.Add(new OctreeNode(new Vec3(origin.x - extents.x, origin.y + extents.y, origin.z - extents.z), extents));
        Children.Add(new OctreeNode(new Vec3(origin.x - extents.x, origin.y + extents.y, origin.z + extents.z), extents));
        Children.Add(new OctreeNode(new Vec3(origin.x + extents.x, origin.y - extents.y, origin.z - extents.z), extents));
        Children.Add(new OctreeNode(new Vec3(origin.x + extents.x, origin.y - extents.y, origin.z + extents.z), extents));
        Children.Add(new OctreeNode(new Vec3(origin.x + extents.x, origin.y + extents.y, origin.z - extents.z), extents));
        Children.Add(new OctreeNode(new Vec3(origin.x + extents.x, origin.y + extents.y, origin.z + extents.z), extents));
    }

    // Further detections within child nodes 
    private void SendObjectsToChildren() {
        List<GameObject> delist = new List<GameObject>();
        // Iterate through the objects in this list and see which child they belong to
        foreach ( GameObject go in Objects) {
            Bounds b = go.GetComponent<Renderer>().bounds;
            Vec3 goCentre = new Vec3(b.center);
            Vec3 goExtents = new Vec3(b.extents);
            
            foreach (OctreeNode child in Children) {
                if (child.BBox.containsObject(goCentre, goExtents)) {
                    if (child.AddObject(go)) {
                      // object successfully added to child object
                      delist.Add(go);
                      break; // Break out and return to go loop
                    } 
                } 
            }
        }
        foreach (GameObject go in delist) {
            Objects.Remove(go);
        }
    }

    public bool AddObject(GameObject aObject) {
        bool objectAdded = false;
        // If node has children attempt to add object to child of node
        if (Children != null && Children.Count > 0) {
            foreach (OctreeNode child in Children) {
                objectAdded = child.AddObject(aObject);
                if (objectAdded) {
                    break;
                }
            }
        }
        // if object not added then continue to attempt to add object to this node
        if (objectAdded != true) {
            if (Objects == null) {
                Objects = new List<GameObject>();
            }

            Bounds b = aObject.GetComponent<Renderer>().bounds;
            Vec3 goCentre = new Vec3(b.center);
            Vec3 goExtents = new Vec3(b.extents);
            if (BBox.containsObject(goCentre, goExtents)) {
                Objects.Add(aObject);
                objectAdded = true;
                // if our object count exceeds a certain amount then we initialise children and redistribute objects 
                if (Objects.Count >= 3 && Children == null) {
                    MakeChildren();
                    SendObjectsToChildren();
                }
            }
        }

        return objectAdded;
    }

    public bool RemoveObject(GameObject aObject, bool fromChildren = true) {
        if (Objects != null) {
            if (Objects.Remove(aObject)) {
                return true;
            }
        }

        if (fromChildren && Children != null) {
            foreach (OctreeNode child in Children) {
                if (child.RemoveObject(aObject)) {
                    return true;
                }
            }
        }
        return false;
    }
    
    // Draw the parent bound inbox
    public void Draw() {
        BBox.Draw();
        if (Children != null) {
            // Draw each child bounds
            foreach (OctreeNode child in Children) {
                child.Draw();
            } 
        }
    }
    
    // Testing to see if the collision was successful and editing the list depending on result
    public int PerformCollisionTest() {
        int numTest = 0;
        // Step through children 
        if (Children != null && Children.Count > 0 ) {
            foreach (OctreeNode child in Children) {
               numTest += child.PerformCollisionTest();
            }
        }

        if (Objects != null && Objects.Count > 1) {
            
            List<GameObject> delist = new List<GameObject>();
            List<GameObject> po = (Parent == null) ? null : Parent.GetObjects();
            List<GameObject> collisionList = new List<GameObject>();
            
            if (po != null) {
                collisionList.AddRange(po);
            }
            collisionList.AddRange(Objects);

            for (int i = 0; i < collisionList.Count-1; ++i) { 
                // Test bounds againts object in list
                Bounds o1 = collisionList[i].GetComponent<Renderer>().bounds; 
                Vec3 o1Centre = new Vec3(o1.center);
                Vec3 o1Extents = new Vec3(o1.extents);
                for (int j = i+1; j < collisionList.Count; j++) { 
                    if (collisionList[i] != collisionList[j]) { 
                        Bounds o2 = Objects[j].GetComponent<Renderer>().bounds;
                        Vec3 o2Centre = new Vec3(o2.center);
                        Vec3 o2Extents = new Vec3(o2.extents);

                        numTest++;
                        
                        //test two objects for collision
                        if (o1Centre.x - o1Extents.x < o2Centre.x + o2Extents.x && o1Centre.x + o1Extents.x > o2Centre.x - o2Extents.x &&
                            o1Centre.y - o1Extents.y < o2Centre.y + o2Extents.y && o1Centre.y + o1Extents.y > o2Centre.y - o2Extents.y &&
                            o1Centre.z - o1Extents.z < o2Centre.z + o2Extents.z && o1Centre.z + o1Extents.z > o2Centre.z - o2Extents.z) {
                            // object in collision with another object
                            delist.Add(collisionList[i]);
                            delist.Add(collisionList[j]);
                        }
                    }
                }
            }
            //Removing successful collisions from the scene both bullet and clay pidgeon
            if (delist.Count > 0) {
                foreach (GameObject go in delist) {
                    if (!Objects.Remove(go)) {
                        if (Parent != null) {
                            Parent.RemoveObject(go, false);
                            
                        }
                    }
                    GameObject.Destroy(go);
                    collision = true;
                    collisionCount += 0.5f; // Dividing by two to remove bullet from collision count
                }
            }
        }

        return numTest;
    }
}

