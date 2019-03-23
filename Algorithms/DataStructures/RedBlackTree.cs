public class RedBlackTreeNode<T> {
    public T data;
    public RedBlackTreeNode<T> left;
    public RedBlackTreeNode<T> right;
    public RedBlackTreeNode<T> parent;
    public RedBlackTreeNode<T> previous;
    public RedBlackTreeNode<T> next;
    public bool red;
}

public class RedBlackTree<T> {
    public RedBlackTreeNode<T> root { get; set; }

    public RedBlackTreeNode<T> Insert(RedBlackTreeNode<T> root, T successorData) {
        RedBlackTreeNode<T> successorNode = new RedBlackTreeNode<T> { data = successorData };

        RedBlackTreeNode<T> parent = null;

        if (root != null) {
            successorNode.previous = root;
            successorNode.next = root.next;
            if (root.next != null)
                root.next.previous = successorNode;

            root.next = successorNode;

            if (root.right != null) {
                root = GetFirst(root.right);
                root.left = successorNode;
            } else {
                root.right = successorNode;
            }
            parent = root;
        } else if (this.root != null) {
            root = GetFirst(root);
            successorNode.next = root;
            root.previous = successorNode;
            root.left = successorNode;
            parent = root;
        } else {
            this.root = successorNode;
        }

        successorNode.parent = parent;
        successorNode.red = true;

        RedBlackTreeNode<T> grandma;
        RedBlackTreeNode<T> aunty;

        root = successorNode;

        while (parent != null && parent.red) {
            grandma = parent.parent;
            if (parent == grandma.left) {
                aunty = grandma.right;
                if (aunty != null && aunty.red) {
                    parent.red = false;
                    aunty.red = false;
                    grandma.red = true;
                    root = grandma;
                } else {
                    if (root == parent.right) {
                        RotateLeft(parent);
                        root = parent;
                        parent = root.parent;
                    }
                    parent.red = false;
                    grandma.red = true;
                    RotateRight(grandma);
                }
            } else {
                aunty = grandma.left;
                if (aunty != null && aunty.red) {
                    parent.red = false;
                    aunty.red = false;
                    grandma.red = true;
                    root = grandma;
                } else {
                    if (root == parent.left) {
                        RotateRight(parent);
                        root = parent;
                        parent = root.parent;
                    }
                    parent.red = false;
                    grandma.red = true;
                    RotateRight(grandma);
                }
            }
            parent = root.parent;
        }
        this.root.red = false;
        return successorNode;
    }

    public void Remove(RedBlackTreeNode<T> root) {
        if (root.next != null)
            root.next.previous = root.previous;
        if (root.previous != null)
            root.previous.next = root.next;

        RedBlackTreeNode<T> original = root;
        RedBlackTreeNode<T> parent = root.parent;
        RedBlackTreeNode<T> left = root.left;
        RedBlackTreeNode<T> right = root.right;

        RedBlackTreeNode<T> next;
        if (left == null)
            next = right;
        else if (right == null)
            next = left;
        else
            next = GetFirst(right);

        if (parent != null) {
            if (parent.left == root)
                parent.left = next;
            else
                parent.right = next;
        } else {
            this.root = next;
        }

        bool red;
        if (left != null && right != null) {
            red = next.red;
            next.red = root.red;
            next.left = left;
            left.parent = next;

            if (next != right) {
                parent = next.parent;
                next.parent = root.parent;

                root = next.right;
                parent.left = root;

                next.right = right;
                right.parent = next;
            } else {
                next.parent = parent;
                parent = next;
                root = next.right;
            }
        } else {
            red = root.red;
            root = next;
        }

        if (root != null)
            root.parent = parent;

        if (red)
            return;

        if (root != null && root.red) {
            root.red = false;
            return;
        }

        RedBlackTreeNode<T> sibling = null;
        do {
            if (root == this.root)
                break;

            if (root == parent.left) {
                sibling = parent.right;
                if (sibling.red) {
                    sibling.red = false;
                    parent.red = true;
                    RotateLeft(parent);
                    sibling = parent.right;
                }

                if ((sibling.left != null && sibling.left.red) || (sibling.right != null && sibling.right.red)) {
                    if (sibling.right == null || !sibling.right.red) {
                        sibling.left.red = false;
                        sibling.red = true;
                        RotateRight(sibling);
                        sibling = parent.right;
                    }
                    sibling.red = parent.red;
                    parent.red = sibling.right.red = false;
                    RotateLeft(parent);
                    root = this.root;
                    break;
                }
            } else {
                sibling = parent.left;
                if (sibling.red) {
                    sibling.red = false;
                    parent.red = true;
                    RotateRight(parent);
                    sibling = parent.left;
                }
                if ((sibling.left != null && sibling.left.red) || (sibling.right != null && sibling.right.red)) {
                    if (sibling.left == null || !sibling.left.red) {
                        sibling.right.red = false;
                        sibling.red = true;
                        RotateLeft(sibling);
                        sibling = parent.left;
                    }
                    sibling.red = parent.red;
                    parent.red = sibling.left.red = false;
                    RotateRight(parent);
                    root = this.root;
                    break;
                }
            }

            sibling.red = true;
            root = parent;
            parent = parent.parent;
        } while (!root.red);

        if (root != null)
            root.red = false;
    }

    public static RedBlackTreeNode<T> GetFirst(RedBlackTreeNode<T> root) {
        if (root == null)
            return null;

        while (root.left != null)
            root = root.left;

        return root;
    }

    public static RedBlackTreeNode<T> GetLast(RedBlackTreeNode<T> root) {
        if (root == null)
            return null;

        while (root.right != null)
            root = root.right;

        return root;
    }

    private void RotateLeft(RedBlackTreeNode<T> root) {
        RedBlackTreeNode<T> p = root;
        RedBlackTreeNode<T> q = root.right;
        RedBlackTreeNode<T> parent = p.parent;

        if (parent != null) {
            if (parent.left == p)
                parent.left = q;
            else
                parent.right = q;
        } else {
            this.root = q;
        }

        q.parent = parent;
        p.parent = q;
        p.right = q.left;
        if (p.right != null)
            p.right.parent = p;
        q.left = p;
    }

    private void RotateRight(RedBlackTreeNode<T> root) {
        RedBlackTreeNode<T> p = root;
        RedBlackTreeNode<T> q = root.left;
        RedBlackTreeNode<T> parent = p.parent;

        if (parent != null) {
            if (parent.left == p)
                parent.left = q;
            else
                parent.right = q;
        } else {
            this.root = q;
        }

        q.parent = parent;
        p.parent = q;
        p.left = q.right;
        if (p.left != null)
            p.left.parent = p;
        q.right = p;
    }
}