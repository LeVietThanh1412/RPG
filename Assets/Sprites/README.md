# Sprite Organization Guide

## 📁 Cấu trúc Sprites đã được tổ chức

### 🧑 Characters (`Sprites/Characters/`)
- **Idle.png** - Character idle animations (4 directions)
- **Walk.png** - Character walking animations (4 directions)

**Cách sử dụng:**
- Slice sprites thành các frame riêng biệt
- Tạo Animator Controller với states: Idle Down/Up/Left/Right, Walk Down/Up/Left/Right
- Sử dụng cho Player hoặc NPCs

### 🐄 Animals (`Sprites/Animals/`)
- **Baby Chicken Yellow.png** - Gà con màu vàng
- **Chicken Blonde Green.png** - Gà trưởng thành màu xanh
- **Chicken Red.png** - Gà màu đỏ
- **Female Cow Brown.png** - Bò cái màu nâu
- **Male Cow Brown.png** - Bò đực màu nâu

**Cách sử dụng:**
- Sử dụng cho farm animals trong game
- Có thể tạo animation đơn giản hoặc để static
- Add vào NPCController để tạo farm atmosphere

### 🏗️ Environment

#### Buildings (`Sprites/Environment/Buildings/`)
- **House.png** - Nhà ở
- **Fence's copiar.png** - Hàng rào

**Cách sử dụng:**
- Slice để tạo building tiles
- Sử dụng trong Tilemap system
- Tạo collision cho buildings

#### Roads (`Sprites/Environment/Roads/`)
- **Road copiar.png** - Đường đi

**Cách sử dụng:**
- Slice thành road tiles
- Sử dụng trong Tilemap cho pathways

#### Vegetation (`Sprites/Environment/Vegetation/`)
- **Maple Tree.png** - Cây phong
- **Spring Crops.png** - Cây trồng mùa xuân

**Cách sử dụng:**
- Trees: Tạo static obstacles hoặc decoration
- Crops: Có thể tạo animation growth stages

#### Tilesets (`Sprites/Environment/Tilesets/`)
- **Tileset Spring.png** - Tileset chính cho môi trường
- **Walls and Floors copiar.png** - Tường và sàn

**Cách sử dụng:**
- Slice thành individual tiles
- Import vào Tile Palette
- Sử dụng với Tilemap Renderer

### 🎒 Items

#### Food (`Sprites/Items/Food/`)
- **Plates.png** - Đĩa thức ăn

**Cách sử dụng:**
- Slice thành individual food items
- Tạo ItemData ScriptableObjects
- Sử dụng cho consumable items

#### Furniture (`Sprites/Items/Furniture/`)
- **Interior.png** - Nội thất

**Cách sử dụng:**
- Slice thành furniture pieces
- Có thể dùng để decorate interiors
- Hoặc tạo placeable items

## 🔧 Hướng dẫn Setup

### 1. Sprite Settings
```
Sprite Mode: Multiple (cho sprite sheets)
Pixels Per Unit: 16 (cho pixel perfect)
Filter Mode: Point (no blur)
Compression: None
```

### 2. Slicing Sprites
1. Select sprite → Sprite Editor
2. Slice → Type: Grid By Cell Size
3. Pixel Size: 16x16 (hoặc theo kích thước actual)
4. Apply

### 3. Animation Setup
1. Drag sprites vào scene để tạo animation
2. Tạo Animator Controller
3. Setup animation states với parameters từ PlayerController

### 4. Tilemap Setup
1. GameObject → 2D Object → Tilemap → Rectangular
2. Window → 2D → Tile Palette
3. Drag sliced sprites vào Tile Palette
4. Paint tiles vào Tilemap

## 💡 Tips sử dụng

### Character Animation
- Idle: Frame 0 của mỗi direction
- Walk: Frames 1-3 của mỗi direction
- Loop walk animations

### Environment Design
- Sử dụng Tilemap Collider 2D cho collision
- Layer ordering: Background → Environment → Characters → UI

### Performance
- Use Sprite Atlas để optimize draw calls
- Set appropriate texture sizes
- Use object pooling cho items

### Item Integration
```csharp
// Example: Tạo food item
ItemData carrot = ScriptableObject.CreateInstance<ItemData>();
carrot.itemName = "Carrot";
carrot.icon = carrotSprite; // Từ Plates.png
carrot.itemType = ItemType.Consumable;
carrot.healthRestore = 10;
```

## 🎨 Art Style Guidelines
- Pixel perfect rendering
- 16x16 base tile size
- Limited color palette (farm theme)
- Top-down perspective
- Consistent lighting direction

Sprites đã được tổ chức sẵn để sử dụng trong RPG framework!
