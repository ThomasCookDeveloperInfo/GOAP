using System;
using System.Collections.Generic;

public class Fortunes {
    public LinkedList<Edge> GenerateVoronoi(List<FortuneSite> sites, double minX, double minY, double maxX, double maxY) {
        MinHeap<FortuneEvent> eventQueue = new MinHeap<FortuneEvent>(5 * sites.Count);
        foreach (FortuneSite site in sites) {
            eventQueue.Insert(new FortuneSiteEvent(site));
        }

        BeachLine beachLine = new BeachLine();
        LinkedList<Edge> edges = new LinkedList<Edge>();
        HashSet<FortuneCircleEvent> deleted = new HashSet<FortuneCircleEvent>();
        
        while (eventQueue.count != 0) {
            FortuneEvent fortuneEvent = eventQueue.Pop();

            if (fortuneEvent is FortuneSiteEvent) {
                beachLine.AddSection((FortuneSiteEvent)fortuneEvent, eventQueue, deleted, edges);
            } else {
                if (deleted.Contains((FortuneCircleEvent) fortuneEvent)) {
                    deleted.Remove((FortuneCircleEvent)fortuneEvent);
                } else {
                    beachLine.RemoveBeachSection((FortuneCircleEvent)fortuneEvent, eventQueue, deleted, edges);
                }
            }
        }

        LinkedListNode<Edge> edgeNode = edges.First;
        while (edgeNode != null) {
            Edge edge = edgeNode.Value;
            LinkedListNode<Edge> nextNode = edgeNode.Next;
            
            if (!ClipEdge(edge, minX, minY, maxX, maxY)) {
                edges.Remove(edgeNode);
            }

            edgeNode = nextNode;
        }

        return edges;
    }

    private static bool ClipEdge(Edge edge, double minX, double minY, double maxX, double maxY) {
        bool accept = false;

        if (edge.end == null) {
            accept = ClipRay(edge, minX, minY, maxX, maxY);
        } else {
            int start = ComputeOutCode(edge.start.x, edge.start.y, minY, minY, maxX, maxY);
            int end = ComputeOutCode(edge.end.x, edge.end.y, minX, minY, maxX, maxY);

            while (true) {
                if ((start | end) == 0) {
                    accept = true;
                    break;
                }
                if ((start & end) == 0) {
                    break;
                }

                double x = -1;
                double y = -1;

                int outcode = start != 0 ? start : end;

                if ((outcode & 0x8) != 0) {
                    x = edge.start.x + (edge.end.x - edge.start.x) * (maxY - edge.start.y) / (edge.end.y - edge.start.y);
                    y = maxY;
                } else if ((outcode & 0x4) != 0) {
                    x = edge.start.x + (edge.end.x - edge.start.x) * (minY - edge.start.y) / (edge.end.y - edge.start.y);
                    y = minY;
                } else if ((outcode & 0x2) != 0) {
                    y = edge.start.y + (edge.end.y - edge.start.y) * (maxX - edge.start.x) / (edge.end.x - edge.start.x);
                    x = maxX;
                } else if ((outcode & 0x1) != 0) {
                    y = edge.start.y + (edge.end.y - edge.start.y) * (minX - edge.start.x) / (edge.end.x - edge.start.x);
                    x = minX;
                }

                if (outcode == start) {
                    edge.start = new Point(x, y);
                    start = ComputeOutCode(x, y, minX, minY, maxX, maxY);
                } else {
                    edge.end = new Point(x, y);
                    end = ComputeOutCode(x, y, minX, minY, maxX, maxY);
                }
            }
        }

        if (edge.neighbour != null) {
            bool valid = ClipEdge(edge.neighbour, minX, minY, maxX, maxY);

            if (accept && valid) {
                edge.start = edge.neighbour.end;
            }

            if (!accept && valid) {
                edge.start = edge.neighbour.end;
                edge.end = edge.neighbour.start;
                accept = true;
            }
        }

        return accept;
    }

    private static bool ClipRay(Edge edge, double minX, double minY, double maxX, double maxY) {
        Point start = edge.start;

        if (edge.slopeRise.ApproxEqual(0)) {
            if (!Within(start.y, minY, maxY))
                return false;
            if (edge.slopeRun > 0 && start.x > maxX)
                return false;
            if (edge.slopeRun < 0 && start.x < minX)
                return false;

            if (Within(start.x, minX, maxX)) {
                if (edge.slopeRun > 0)
                    edge.end = new Point(maxX, start.y);
                else
                    edge.end = new Point(minX, start.y);
            } else {
                if (edge.slopeRun > 0) {
                    edge.start = new Point(minX, start.y);
                    edge.end = new Point(maxX, start.y);
                } else {
                    edge.start = new Point(maxX, start.y);
                    edge.end = new Point(minX, start.y);
                }
            }
            return true;
        }

        if (edge.slopeRun.ApproxEqual(0)) {
            if (start.x < minX || start.x > maxX)
                return false;
            if (edge.slopeRise > 0 && start.y > maxY)
                return false;
            if (edge.slopeRise < 0 && start.y < minY)
                return false;

            if (Within(start.y, minY, maxY)) {
                if (edge.slopeRise > 0)
                    edge.end = new Point(start.x, maxY);
                else
                    edge.end = new Point(start.x, minY);
            } else {
                if (edge.slopeRise > 0) {
                    edge.start = new Point(start.x, minY);
                    edge.end = new Point(start.x, maxY);
                } else {
                    edge.start = new Point(start.x, maxY);
                    edge.end = new Point(start.x, minY);
                }
            }

            return true;
        }

        Point topX = new Point(CalcX(edge.slope.Value, maxY, edge.intercept.Value), maxY);
        Point bottomX = new Point(CalcX(edge.slope.Value, minY, edge.intercept.Value), minY);
        Point leftY = new Point(minX, CalcY(edge.slope.Value, minX, edge.intercept.Value));
        Point rightY = new Point(maxX, CalcY(edge.slope.Value, maxX, edge.intercept.Value));

        List<Point> candidates = new List<Point>();
        if (Within(topX.x, minX, maxX))
            candidates.Add(topX);
        if (Within(bottomX.x, minX, maxX))
            candidates.Add(bottomX);
        if (Within(leftY.y, minY, maxY))
            candidates.Add(leftY);
        if (Within(rightY.y, minY, maxY))
            candidates.Add(rightY);

        for (int i = candidates.Count - 1; i >= 0; i--) {
            Point candidate = candidates[i];
            double aX = candidate.x - start.x;
            double aY = candidate.y - start.y;
            if (edge.slopeRun * aX + edge.slopeRise * aY < 0)
                candidates.RemoveAt(i);
        }

        if (candidates.Count == 2) {
            double aX = candidates[0].x - start.x;
            double aY = candidates[0].y - start.y;
            double bX = candidates[1].x - start.x;
            double bY = candidates[1].y - start.y;

            if (aX * aX + aY * aY > bX * bX + bY * bY) {
                edge.start = candidates[1];
                edge.end = candidates[0];
            } else {
                edge.start = candidates[0];
                edge.end = candidates[1];
            }
        }

        if (candidates.Count == 1)
            edge.end = candidates[0];

        return edge.end != null;
    }

    private static int ComputeOutCode(double x, double y, double minX, double minY, double maxX, double maxY) {
        int code = 0;
        if (x < minX) {
            code |= 0x1;
        } else if (x > maxX) {
            code |= 0x2;
        }

        if (y < minY) {
            code |= 0x4;
        } else if (y > maxY) {
            code |= 0x8;
        }

        return code;
    }

    private static bool Within(double x, double a, double b) {
        return x.ApproxGreaterThanOrEqualTo(a) && x.ApproxLessThanOrEqualTo(b);
    }

    private static double CalcY(double m, double x, double b) {
        return m * x + b;
    }

    private static double CalcX(double m, double y, double b) {
        return (y - b) / m;
    }
}

public class FortuneSite {
    public readonly double x;
    public readonly double y;
    public readonly List<Edge> cell;
    public readonly List<FortuneSite> neighbours;

    public FortuneSite(double x, double y) {
        this.x = x;
        this.y = y;
        this.cell = new List<Edge>();
        this.neighbours = new List<FortuneSite>();
    }
}

public class Edge {
    public readonly FortuneSite left;
    public readonly FortuneSite right;
    public readonly double slopeRise;
    public readonly double slopeRun;
    public readonly double? slope;
    public readonly double? intercept;

    public Edge neighbour;
    public Point start;
    public Point end;

    public Edge(Point start, FortuneSite left, FortuneSite right) {
        this.start = start;
        this.left = left;
        this.right = right;

        if (this.left == null || this.right == null)
            return;

        this.slopeRise = this.left.x - this.right.x;
        this.slopeRun = -(this.left.y - this.right.y);

        if (this.slopeRise.ApproxEqual(0) || this.slopeRun.ApproxEqual(0))
            return;

        this.slope = this.slopeRise / this.slopeRun;
        this.intercept = this.start.y - this.slope * this.start.x;
    }
}

public class Point {
    public readonly double x;
    public readonly double y;

    public Point(double x, double y) {
        this.x = x;
        this.y = y;
    }
}

public interface FortuneEvent : IComparable<FortuneEvent> {
    double x { get; }
    double y { get; }
}

public class FortuneSiteEvent : FortuneEvent {
    public readonly FortuneSite site;

    public double x { get { return this.site.x; } }
    public double y { get { return this.site.y; } }

    public FortuneSiteEvent(FortuneSite site) {
        this.site = site;
    }

    public int CompareTo(FortuneEvent other) {
        int comparison = this.y.CompareTo(other.y);
        return comparison == 0 ? this.x.CompareTo(other.x) : comparison;
    }
}

public class FortuneCircleEvent : FortuneEvent {
    public readonly Point lowest;
    public readonly double yCenter;

    public RedBlackTreeNode<BeachSection> toDelete;

    public double x { get { return this.lowest.x; } }
    public double y { get { return this.lowest.y; } }

    public FortuneCircleEvent(Point lowest, double yCenter, RedBlackTreeNode<BeachSection> toDelete) {
        this.lowest = lowest;
        this.yCenter = yCenter;
        this.toDelete = toDelete;
    }

    public int CompareTo(FortuneEvent other) {
        int comparison = this.y.CompareTo(other.y);
        return comparison == 0 ? this.x.CompareTo(other.x) : comparison;
    }
}

public class BeachSection {
    public readonly FortuneSite site;
    public Edge edge { get; set; }
    public FortuneCircleEvent circleEvent { get; set; }

    public BeachSection(FortuneSite site) {
        this.site = site;
        this.circleEvent = null;
    }
}

public class BeachLine {
    private readonly RedBlackTree<BeachSection> sections;

    public BeachLine() {
        this.sections = new RedBlackTree<BeachSection>();
    }

    public void AddSection(FortuneSiteEvent siteEvent, MinHeap<FortuneEvent> eventQueue, HashSet<FortuneCircleEvent> deleted, LinkedList<Edge> edges) {
        FortuneSite site = siteEvent.site;
        double x = site.x;
        double directrix = site.y;

        RedBlackTreeNode<BeachSection> leftSection = null;
        RedBlackTreeNode<BeachSection> rightSection = null;
        RedBlackTreeNode<BeachSection> root = sections.root;

        while (root != null && leftSection != null && rightSection != null) {
            double distanceLeft = LeftBreakpoint(root, directrix) - x;
            if (distanceLeft > 0) {
                if (root.left == null)
                    rightSection = root;
                else
                    root = root.left;

                continue;
            }

            double distanceRight = x - RightBreakpoint(root, directrix);
            if (distanceRight > 0) {
                if (root.right == null)
                    leftSection = root;
                root = root.right;

                continue;
            }

            if (distanceLeft.ApproxEqual(0)) {
                leftSection = root.previous;
                rightSection = root;
                continue;
            }

            if (distanceRight.ApproxEqual(0)) {
                leftSection = root;
                rightSection = root.next;
                continue;
            }

            leftSection = rightSection = root;
        }

        BeachSection section = new BeachSection(site);
        RedBlackTreeNode<BeachSection> newSectionNode = sections.Insert(leftSection, section);

        if (leftSection == null && rightSection == null)
            return;

        if (leftSection != null && leftSection == rightSection) {
            if (leftSection.data.circleEvent != null) {
                deleted.Add(leftSection.data.circleEvent);
                leftSection.data.circleEvent = null;
            }

            BeachSection copy = new BeachSection(leftSection.data.site);
            rightSection = sections.Insert(newSectionNode, copy);

            double y = ParabolaMath.EvalParabola(leftSection.data.site.x, leftSection.data.site.y, directrix, x);
            Point intersection = new Point(x, y);

            Edge leftEdge = new Edge(intersection, site, leftSection.data.site);
            Edge rightEdge = new Edge(intersection, leftSection.data.site, site);
            leftEdge.neighbour = rightEdge;

            edges.AddFirst(leftEdge);

            newSectionNode.data.edge = leftEdge;
            rightSection.data.edge = rightEdge;

            leftSection.data.site.neighbours.Add(newSectionNode.data.site);
            newSectionNode.data.site.neighbours.Add(leftSection.data.site);

            CheckCircle(leftSection, eventQueue);
            CheckCircle(rightSection, eventQueue);
        } else if (leftSection != null && rightSection != null) {
            Point start = new Point((leftSection.data.site.x + site.x) / 2, double.MinValue);
            Edge infiniteEdge = new Edge(start, leftSection.data.site, site);
            Edge newEdge = new Edge(start, site, leftSection.data.site);

            newEdge.neighbour = infiniteEdge;
            edges.AddFirst(newEdge);

            leftSection.data.site.neighbours.Add(newSectionNode.data.site);
            newSectionNode.data.site.neighbours.Add(leftSection.data.site);

            newSectionNode.data.edge = newEdge;
        } else if (leftSection != null && leftSection != rightSection) {
            if (leftSection.data.circleEvent != null) {
                deleted.Add(leftSection.data.circleEvent);
                leftSection.data.circleEvent = null;
            }

            if (rightSection.data.circleEvent != null) {
                deleted.Add(rightSection.data.circleEvent);
                rightSection.data.circleEvent = null;
            }

            FortuneSite leftSite = leftSection.data.site;
            double aX = leftSite.x;
            double aY = leftSite.y;
            double bX = site.x - aX;
            double bY = site.y - aY;

            FortuneSite rightSite = rightSection.data.site;
            double cX = rightSite.x - aX;
            double cY = rightSite.y - aY;
            double delta = bX * cY - bY * cX;
            double magnitudeB = bX * bX + bY * bY;
            double magnitudeC = cX * cX + cY * cY;

            Point vertex = new Point(
                (cY * magnitudeB - bY * magnitudeC) / (2 * delta) + aX,
                (bX * magnitudeC - cX * magnitudeB) / (2 * delta) + aY
            );

            rightSection.data.edge.end = vertex;

            newSectionNode.data.edge = new Edge(vertex, site, leftSection.data.site);
            rightSection.data.edge = new Edge(vertex, rightSection.data.site, site);

            edges.AddFirst(newSectionNode.data.edge);
            edges.AddFirst(rightSection.data.edge);

            newSectionNode.data.site.neighbours.Add(leftSection.data.site);
            leftSection.data.site.neighbours.Add(newSectionNode.data.site);

            newSectionNode.data.site.neighbours.Add(rightSection.data.site);
            rightSection.data.site.neighbours.Add(newSectionNode.data.site);

            CheckCircle(leftSection, eventQueue);
            CheckCircle(rightSection, eventQueue);
        }
    }

    public void RemoveBeachSection(FortuneCircleEvent circle, MinHeap<FortuneEvent> eventQueue, HashSet<FortuneCircleEvent> deleted, LinkedList<Edge> edges) {
        RedBlackTreeNode<BeachSection> section = circle.toDelete;
        double x = circle.x;
        double y = circle.yCenter;
        Point vertex = new Point(x, y);

        List<RedBlackTreeNode<BeachSection>> toBeRemoved = new List<RedBlackTreeNode<BeachSection>>();

        RedBlackTreeNode<BeachSection> previous = section.previous;
        while (previous.data.circleEvent != null && (x - previous.data.circleEvent.x).ApproxEqual(0) && (y - previous.data.circleEvent.y).ApproxEqual(0)) {
            toBeRemoved.Add(previous);
            previous = previous.previous;
        }

        RedBlackTreeNode<BeachSection> next = section.next;
        while (next.data.circleEvent != null && (x - next.data.circleEvent.x).ApproxEqual(0) && (y - next.data.circleEvent.y).ApproxEqual(0)) {
            toBeRemoved.Add(next);
            next = next.next;
        }

        section.data.edge.end = vertex;
        section.next.data.edge.end = vertex;
        section.data.circleEvent = null;

        foreach (RedBlackTreeNode<BeachSection> toRemove in toBeRemoved) {
            toRemove.data.edge.end = vertex;
            toRemove.next.data.edge.end = vertex;
            deleted.Add(toRemove.data.circleEvent);
            toRemove.data.circleEvent = null;
        }

        if (previous.data.circleEvent != null) {
            deleted.Add(previous.data.circleEvent);
            previous.data.circleEvent = null;
        }
        if (next.data.circleEvent != null) {
            deleted.Add(next.data.circleEvent);
            next.data.circleEvent = null;
        }

        Edge newEdge = new Edge(vertex, next.data.site, previous.data.site);
        next.data.edge = newEdge;
        edges.AddFirst(newEdge);

        previous.data.site.neighbours.Add(next.data.site);
        next.data.site.neighbours.Add(previous.data.site);

        sections.Remove(section);
        foreach (RedBlackTreeNode<BeachSection> toRemove in toBeRemoved) {
            sections.Remove(toRemove);
        }

        CheckCircle(previous, eventQueue);
        CheckCircle(next, eventQueue);
    }

    private static void CheckCircle(RedBlackTreeNode<BeachSection> section, MinHeap<FortuneEvent> eventQueue) {
        RedBlackTreeNode<BeachSection> left = section.previous;
        RedBlackTreeNode<BeachSection> right = section.next;
        if (left == null || right == null)
            return;

        FortuneSite leftSite = left.data.site;
        FortuneSite centerSite = section.data.site;
        FortuneSite rightSite = right.data.site;

        if (leftSite == rightSite)
            return;


        double bX = centerSite.x;
        double bY = centerSite.y;
        double aX = leftSite.x - bX;
        double aY = leftSite.y - bY;
        double cX = rightSite.x - bX;
        double cY = rightSite.y - bY;

        double delta = aX * cY - aY * cX;
        if (delta.ApproxGreaterThanOrEqualTo(0))
            return;

        double magnitudeA = aX * aX + aY * aY;
        double magnitudeC = cX * cX + cY * cY;

        double x = (cY * magnitudeA - aY * magnitudeC) / (2 * delta);
        double y = (aX * magnitudeC - cX * magnitudeA) / (2 * delta);

        double yCenter = y + bY;

        FortuneCircleEvent circleEvent = new FortuneCircleEvent(
            new Point(x + bX, yCenter + Math.Sqrt(x * x + y * y)),
            yCenter, section
        );

        section.data.circleEvent = circleEvent;
        eventQueue.Insert(circleEvent);
    }

    private static double LeftBreakpoint(RedBlackTreeNode<BeachSection> node, double directrix) {
        RedBlackTreeNode<BeachSection> leftNode = node.previous;

        if ((node.data.site.y - directrix).ApproxEqual(0))
            return node.data.site.x;

        if (leftNode == null)
            return double.NegativeInfinity;

        if ((leftNode.data.site.y - directrix).ApproxEqual(0))
            return leftNode.data.site.x;

        FortuneSite site = node.data.site;
        FortuneSite leftSite = leftNode.data.site;

        return ParabolaMath.IntersectParabola(leftSite.x, leftSite.y, site.x, site.y, directrix);
    }

    private static double RightBreakpoint(RedBlackTreeNode<BeachSection> node, double directrix) {
        RedBlackTreeNode<BeachSection> rightNode = node.next;

        if ((node.data.site.y - directrix).ApproxEqual(0))
            return node.data.site.x;

        if (rightNode == null)
            return double.PositiveInfinity;

        if ((rightNode.data.site.y - directrix).ApproxEqual(0))
            return rightNode.data.site.x;

        FortuneSite site = node.data.site;
        FortuneSite rightSite = rightNode.data.site;

        return ParabolaMath.IntersectParabola(site.x, site.y, rightSite.x, rightSite.y, directrix);
    }
}