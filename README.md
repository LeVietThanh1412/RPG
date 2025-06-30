# 🎮 Unity 2D RPG Framework - Hướng dẫn Việt Nam

## 📋 Thông tin project
- **Tác giả**: Phát triển bởi bạn (Lê Thành)
- **Engine**: Unity 2022.3+ LTS
- **Thể loại**: 2D Top-Down RPG Framework
- **Ngôn ngữ**: C# với hướng dẫn tiếng Việt chi tiết
- **Mục đích**: Framework cơ bản để tạo game 2D RPG, thích hợp cho người mới bắt đầu

## 🎯 Tính năng chính
- ✅ **Player Movement**: Di chuyển 8 hướng mượt mà với WASD/Arrow keys
- ✅ **Animation System**: Hệ thống animation với 4 hướng (Up, Down, Left, Right)
- ✅ **Inventory System**: Hệ thống kho đồ hoàn chình với UI drag & drop
- ✅ **NPC Interaction**: Tương tác với NPC, hệ thống dialogue
- ✅ **Camera Follow**: Camera tự động theo player
- ✅ **Map System**: Tạo map với Tilemap và Sorting Layer
- ✅ **Sound System**: Quản lý âm thanh background và SFX

## 🚀 Hướng dẫn cài đặt nhanh

### Bước 1: Mở project
1. Mở Unity Hub
2. Click "Open" → Chọn thư mục project
3. Đợi Unity import assets (có thể mất vài phút)

### Bước 2: Fix lỗi nếu có
Nếu Console có lỗi đỏ/vàng, chạy lệnh này **1 lần duy nhất**:
```
Unity Menu → RPG Tools → 🚀 ULTIMATE CONSOLE FIX (Fix All Errors)
```

### Bước 3: Test game
1. Mở scene `Assets/Scenes/MainScene`
2. Nhấn **Play**
3. Dùng **WASD** để di chuyển player

## 👤 Hướng dẫn tạo Player từ đầu

### Bước 1: Tạo GameObject Player
```
1. Hierarchy → Right-click → Create Empty
2. Đổi tên thành "Player"
3. Set Position = (0, 0, 0)
4. Set Tag = "Player" (quan trọng!)
```

### Bước 2: Thêm Components cần thiết
```
5. Add Component → Sprite Renderer
6. Add Component → Rigidbody2D
   - Set Gravity Scale = 0 (top-down game)
   - Check Freeze Rotation Z
7. Add Component → Box Collider 2D
   - Adjust size cho phù hợp với sprite
8. Add Component → Animator
```

### Bước 3: Setup Sprite và Animation
```
9. Kéo sprite player vào Sprite Renderer
10. Tạo Animator Controller:
    - Right-click Assets → Create → Animator Controller
    - Đặt tên "PlayerAnimatorController"
    - Kéo vào Animator Component của Player
```

### Bước 4: Thêm Movement Script
```
11. Add Component → Player Controller (script có sẵn)
```

### Bước 5: Setup Animation Parameters
Mở Animator window và thêm 4 parameters:
- **MoveX** (Float) - Giá trị X của input (-1, 0, 1)
- **MoveY** (Float) - Giá trị Y của input (-1, 0, 1) 
- **IsMoving** (Bool) - Player có đang di chuyển không
- **Speed** (Float) - Tốc độ di chuyển hiện tại

## 🎨 Hướng dẫn setup Animation

### Tạo Animation States
1. **Idle State** (mặc định)
   - Kéo sprite idle vào Animator
   - Hoặc tạo Animation clip từ sprite đứng yên

2. **Moving States** (4 hướng)
   - Tạo 4 states: Moving_Up, Moving_Down, Moving_Left, Moving_Right
   - Mỗi state gán Animation clip tương ứng

### Setup Transitions
```
Idle → Moving_Up: IsMoving = true AND MoveY > 0.5
Idle → Moving_Down: IsMoving = true AND MoveY < -0.5
Idle → Moving_Left: IsMoving = true AND MoveX < -0.5
Idle → Moving_Right: IsMoving = true AND MoveX > 0.5

Tất cả Moving states → Idle: IsMoving = false
```

### Cài đặt Transition
- **Has Exit Time**: ✅ Bỏ tick (tắt)
- **Fixed Duration**: ✅ Bỏ tick  
- **Transition Duration**: 0.1 - 0.25
- **Interruption Source**: None

## 🗺️ Hướng dẫn tạo Map

### Bước 1: Setup Tilemap
```
1. Hierarchy → Right-click → 2D Object → Tilemap → Rectangular
2. Sẽ tạo ra:
   - Grid (parent object)
   - Tilemap (child object với Tilemap Renderer)
```

### Bước 2: Tạo Tile Palette
```
3. Window → 2D → Tile Palette
4. Click "Create New Palette"
5. Đặt tên palette (vd: "Environment")
6. Kéo sprite tiles vào palette
```

### Bước 3: Vẽ map
```
7. Chọn tool brush trong Tile Palette
8. Chọn tile muốn vẽ
9. Vẽ trên Scene view
```

## 📚 Hệ thống Sorting Layer (7 tầng)

Framework sử dụng 7 Sorting Layer theo thứ tự từ xa đến gần:

### 1. **Ground** (tầng đất)
- **Mục đích**: Nền đất, đường đi, cỏ, sàn nhà
- **Sorting Layer**: 0 (xa nhất)
- **Ví dụ**: Tile grass, stone floor, sand

### 2. **ForceGround** (nền ép xuống)
- **Mục đích**: Các object luôn ở dưới mọi thứ khác
- **Sorting Layer**: 1
- **Ví dụ**: Bóng của object, vũng nước

### 3. **WalkInFront** (player đi phía trước)
- **Mục đích**: Object mà player có thể đi phía trước
- **Sorting Layer**: 2
- **Ví dụ**: Bụi cỏ thấp, thảm, ánh sáng sàn

### 4. **Collision** (va chạm)
- **Mục đích**: Tường, cây, rock - có collider, player không đi qua được
- **Sorting Layer**: 3
- **Ví dụ**: Tree trunk, walls, big rocks

### 5. **Player** (nhân vật)
- **Mục đích**: Player và NPC
- **Order**: 4 (trung tâm)
- **Ví dụ**: Main character, NPCs, enemies

### 6. **WalkInBehind** (player đi phía sau)
- **Mục đích**: Object mà player đi vào sẽ bị che
- **Sorting Layer**: 5
- **Ví dụ**: Tree leaves, rooftops, tán cây

### 7. **Decoration** (trang trí)
- **Mục đích**: Hiệu ứng, UI world, decoration luôn hiển thị trên cùng
- **Sorting Layer**: 6 (gần nhất)
- **Ví dụ**: Particle effects, floating UI, special effects

### Cách setup Sorting Layer:
```
1. Edit → Project Settings → Tags and Layers
2. Mở Sorting Layers
3. Thêm 7 layer theo thứ tự trên
4. Assign cho các Sprite Renderer/Tilemap Renderer
```

## 🎮 Controls mặc định
- **WASD** hoặc **Arrow Keys**: Di chuyển player
- **E**: Tương tác với NPC/Object
- **I**: Mở/đóng Inventory
- **ESC**: Pause menu

## 🛠️ Troubleshooting - Khắc phục lỗi

### ❌ Player không di chuyển được
**Nguyên nhân có thể là:**
- Missing Rigidbody2D component
- Gravity Scale khác 0
- Script PlayerController bị disable
- Không gán Input System

**Cách fix:**
```
RPG Tools → 🚀 ULTIMATE CONSOLE FIX (Fix All Errors)
```

### ❌ Animation không chạy
**Nguyên nhân có thể là:**
- Thiếu Animator Controller
- Thiếu parameters trong Animator
- Transition setup sai

**Cách fix:**
1. Kiểm tra Animator Controller được gán chưa
2. Kiểm tra 4 parameters: MoveX, MoveY, IsMoving, Speed
3. Kiểm tra Transitions có điều kiện đúng

### ❌ Sprite quá nhỏ hoặc bị mờ
**Cách fix:**
```
1. Chọn sprite trong Project
2. Inspector → Import Settings:
   - Pixels Per Unit: 16 hoặc 32
   - Filter Mode: Point (no filter)
   - Compression: None
3. Click Apply
```

### ❌ Tilemap không hiển thị đúng layer
**Cách fix:**
```
1. Chọn Tilemap trong Hierarchy
2. Tilemap Renderer component:
   - Sorting Layer: Chọn layer phù hợp
   - Sorting Layer in Layer: Set thứ tự nếu cần
```

### ❌ Player bị cây/tường che
**Cách fix:**
- Cây/tường: Set Sorting Layer = "Collision" hoặc "WalkInBehind"
- Player: Set Sorting Layer = "Player"
- Camera: Edit → Project Settings → Graphics → Transparency Sort Mode = "Custom Axis" với Y = 1

## 📁 Cấu trúc thư mục

```
Assets/
├── 🎬 Animations/          # Animation clips và controllers
├── 🔊 Audio/               # Nhạc nền và sound effects  
├── 📦 Prefabs/             # Player, NPC, UI prefabs
├── 🎨 Scenes/              # Game scenes
├── 📋 ScriptableObjects/   # Data assets (items, quests)
├── 📝 Scripts/             # Source code
│   ├── 👤 Player/          # Player movement, stats
│   ├── 🎮 Managers/        # Game manager, camera
│   ├── 🎨 UI/              # Interface scripts
│   ├── 🤖 NPCs/            # NPC AI và dialogue
│   ├── 📦 Items/           # Item system
│   ├── 🎒 Inventory/       # Inventory system
│   └── 🔧 Utilities/       # Helper tools
├── ⚙️ Settings/            # Render pipeline, input
└── 🖼️ Sprites/             # Game artwork
```

## 🎯 Phát triển tiếp

### Tính năng có thể mở rộng:
- **Quest System**: Hệ thống nhiệm vụ
- **Battle System**: Chiến đấu turn-based hoặc real-time
- **Save/Load**: Lưu tiến độ game
- **Multiple Scenes**: Nhiều map, chuyển scene
- **Equipment System**: Trang bị vũ khí, áo giáp
- **Skill Tree**: Cây kỹ năng cho player
- **Day/Night Cycle**: Chu kỳ ngày/đêm

### Sprite resources khuyên dùng:
- **Itch.io**: Nhiều asset miễn phí cho 2D RPG
- **OpenGameArt**: Free sprites và tilesets  
- **Kenney.nl**: Asset packs chất lượng cao
- **LPC (Liberated Pixel Cup)**: Character sprites standardized

## 📞 Hỗ trợ

Nếu gặp vấn đề, hãy:
1. **Kiểm tra Console** để xem lỗi cụ thể
2. **Chạy Ultimate Console Fix** trước khi báo lỗi
3. **Kiểm tra Troubleshooting** ở trên
4. **Đọc kỹ hướng dẫn** setup từng bước

---
## 📝 Ghi chú phiên bản

**v1.0 - Framework hoàn chỉnh**
- ✅ Core movement system
- ✅ Animation framework  
- ✅ Inventory system
- ✅ NPC interaction
- ✅ Map creation tools
- ✅ Sorting layer system
- ✅ Documentation tiếng Việt đầy đủ

**Được phát triển với ❤️ bởi Lê Thành**

**Asset game lấy của https://emanuelledev.itch.io/farm-rpg (Bản free)**

*Framework này được thiết kế để giúp người Việt Nam mới học Unity có thể tạo ra game 2D RPG một cách dễ dàng và hiệu quả.*