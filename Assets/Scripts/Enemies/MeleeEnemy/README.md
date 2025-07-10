# Goblin Enemy AI System

This system provides two different approaches for implementing enemy AI behavior:

## Option 1: State Machine Approach (Recommended)

Use `EnemyController` with the state machine system for more complex and extensible AI behavior.

### Setup Instructions:

1. **Add Components to Enemy GameObject:**

   - `EnemyController` script
   - `Rigidbody2D` (required)
   - `Animator` (required)
   - `Collider2D` (for ground detection)

2. **Configure Inspector Settings:**

   - **Movement Settings:**

     - `Move Speed`: How fast the enemy moves (default: 2f)
     - `Patrol Distance`: How far the enemy patrols from start position (default: 5f)
     - `Detection Range`: How far the enemy can detect the player (default: 3f)
     - `Attack Range`: How close the enemy needs to be to attack (default: 1.5f)
     - `Attack Cooldown`: Time between attacks (default: 1.5f)

   - **References:**
     - `Attack Point`: Transform that defines where the attack originates
     - `Player Layer`: Layer mask for player detection
     - `Ground Layer`: Layer mask for ground collision detection

3. **Layer Setup:**
   - Set the player GameObject to a specific layer (e.g., "Player")
   - Set ground objects to a specific layer (e.g., "Ground")
   - Configure the layer masks in the inspector

### States:

- **Idle State**: Enemy stands still when no player is detected
- **Patrol State**: Enemy moves back and forth between patrol points
- **Chase State**: Enemy moves faster towards the detected player
- **Attack State**: Enemy stops and attacks when player is in range

### Debug Visualization:

Enable `Show Debug Gizmos` to see:

- Yellow box: Patrol area
- Red circle: Detection range
- Magenta circle: Attack range
- Green spheres: Patrol points

## Option 2: Simple Movement Approach

Use `EnemyMovement` for a simpler, single-script solution.

### Setup Instructions:

1. **Add Components to Enemy GameObject:**

   - `EnemyMovement` script (automatically adds Rigidbody2D)
   - `Animator` (required)
   - `Collider2D` (for ground detection)

2. **Configure the same settings as above**

### Behavior:

The simple approach uses a state enum internally:

- **Idle**: Stands still initially
- **Patrol**: Moves between patrol points
- **Chase**: Moves faster towards player
- **Attack**: Stops and attacks when in range

## Animation Requirements:

The enemy should have these animation clips:

- `"Idle"`: Standing idle animation
- `"Walking"`: Walking/movement animation
- `"Attack_1"`: Attack animation

## Performance Considerations:

- Both approaches use `Physics2D.OverlapCircle` for player detection
- State machine approach is more modular and easier to extend
- Simple approach is more straightforward for basic AI needs

## Customization:

### Adding New States (State Machine Approach):

1. Create a new state class inheriting from `BaseEnemyState`
2. Implement the required methods (`OnEnter`, `Update`, `FixedUpdate`, `OnExit`)
3. Add the state to the state machine in `EnemyController.InitializeStateMachine()`
4. Add transitions using `stateMachine.AddTransition()`

### Modifying Behavior:

- Adjust the inspector values to change ranges and speeds
- Modify the state classes to change specific behaviors
- Override the `Attack()` method to implement custom damage logic

## Troubleshooting:

1. **Enemy not moving**: Check if `IsGrounded` is true and ground layer is set correctly
2. **Player not detected**: Verify player layer mask and detection range
3. **Animations not playing**: Ensure animation clips are named correctly
4. **Attack not working**: Check attack point position and attack range

## Example Usage:

```csharp
// In your game manager or level setup
public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    private void SpawnEnemy(Vector3 position)
    {
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        // The enemy will automatically start patrolling
    }
}
```
