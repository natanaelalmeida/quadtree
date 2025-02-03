from matplotlib import pyplot as plt


def plot_points(points, ax, color='red', query_patch=None, found_points=None):
    x_values = [point.x for point in points]
    y_values = [point.y for point in points]

    ax.scatter(x_values, y_values, color=color)

    if query_patch:
        rect = plt.Rectangle((query_patch.x, query_patch.y),
                             query_patch.width,
                             query_patch.height,
                             fill=True,
                             color="blue",
                             alpha=0.3,
                             label="Query Region")

        ax.add_patch(rect)

    if found_points:
        x_values = [point.x for point in found_points]
        y_values = [point.y for point in found_points]

        ax.scatter(x_values, y_values,
                   color='green',
                   label="Found Points",
                   zorder=3)

    ax.set_xlim(0, 100)
    ax.set_ylim(0, 100)
    ax.set_title("Quadtree")
    ax.set_xlabel("X")
    ax.set_ylabel("Y")
    ax.legend()

    plt.gca().set_aspect('equal', adjustable='box')
    plt.show()