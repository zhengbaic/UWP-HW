#include "Thunder.h"
#include <algorithm>

USING_NS_CC;

using namespace CocosDenshion;

Scene* Thunder::createScene() {
  // 'scene' is an autorelease object
  auto scene = Scene::create();

  // 'layer' is an autorelease object
  auto layer = Thunder::create();

  // add layer as a child to scene
  scene->addChild(layer);

  // return the scene
  return scene;
}

bool Thunder::init() {
  if (!Layer::init()) {
    return false;
  }

  isMove = false;  // 是否点击飞船
  visibleSize = Director::getInstance()->getVisibleSize();

  // 创建背景
  auto bgsprite = Sprite::create("bg.jpg");
  bgsprite->setPosition(visibleSize / 2);
  bgsprite->setScale(visibleSize.width / bgsprite->getContentSize().width,
                     visibleSize.height / bgsprite->getContentSize().height);
  this->addChild(bgsprite, 0);

  // 创建飞船
  player = Sprite::create("player.png");
  player->setAnchorPoint(Vec2(0.5, 0.5));
  player->setPosition(visibleSize.width / 2, player->getContentSize().height);
  player->setName("player");
  this->addChild(player, 1);

  // 显示陨石和子弹数量
  enemysNum = Label::createWithTTF("enemys: 0", "fonts/arial.TTF", 20);
  enemysNum->setColor(Color3B(255, 255, 255));
  enemysNum->setPosition(50, 60);
  this->addChild(enemysNum, 3);
  bulletsNum = Label::createWithTTF("bullets: 0", "fonts/arial.TTF", 20);
  bulletsNum->setColor(Color3B(255, 255, 255));
  bulletsNum->setPosition(50, 30);
  this->addChild(bulletsNum, 3);

  addEnemy(5);        // 初始化陨石
  preloadMusic();     // 预加载音乐
  playBgm();          // 播放背景音乐
  explosion();        // 创建爆炸帧动画

  // 添加监听器
  addTouchListener();
  addKeyboardListener();
  addCustomListener();

  // 调度器
  schedule(schedule_selector(Thunder::update), 0.04f, kRepeatForever, 0);

  return true;
}

//预加载音乐文件
void Thunder::preloadMusic() {
  // Todo
	auto audio = SimpleAudioEngine::getInstance();
	audio->preloadBackgroundMusic("music/bgm.mp3");
	audio->preloadEffect("music/explore.wav");
	audio->preloadEffect("music/fire.wav");
}

//播放背景音乐
void Thunder::playBgm() {
  // Todo
	SimpleAudioEngine::getInstance()->playBackgroundMusic("music/bgm.mp3", true);
}

//初始化陨石
void Thunder::addEnemy(int n) {
  enemys.clear();
  for (unsigned i = 0; i < 3; ++i) {
    char enemyPath[20];
    sprintf(enemyPath, "stone%d.png", 3 - i);
    double width = visibleSize.width / (n + 1.0),
           height = visibleSize.height - (50 * (i + 1));
    for (int j = 0; j < n; ++j) {
      auto enemy = Sprite::create(enemyPath);
      enemy->setAnchorPoint(Vec2(0.5, 0.5));
      enemy->setScale(0.5, 0.5);
      enemy->setPosition(width * (j + 1), height);
      enemys.push_back(enemy);
      addChild(enemy, 1);
    }
  }
}

// 陨石向下移动并生成新的一行(加分项)
void Thunder::newEnemy() {
  // Todo
	static int stone = 1;
	for (auto it = enemys.begin(); it != enemys.end(); ++it) {
		if (*it != NULL)
			(*it)->setPosition((*it)->getPosition() + Vec2(0, -50));
	}
	char enemyPath[20];
	sprintf(enemyPath, "stone%d.png", stone);
	double width = visibleSize.width / 6.0,
		height = visibleSize.height - 50;
	for (int j = 0; j < 5; ++j) {
		auto enemy = Sprite::create(enemyPath);
		enemy->setAnchorPoint(Vec2(0.5, 0.5));
		enemy->setScale(0.5, 0.5);
		enemy->setPosition(width * j + 65, height);
		enemys.push_back(enemy);
		addChild(enemy, 1);
	}
	stone = stone % 3 + 1;
}

// 移动飞船
void Thunder::movePlane(char c) {
  // Todo
	auto nowPos = player->getPosition();
	switch (c) {
	case 'A':
		player->runAction(MoveBy::create(0.2f, Vec2(-min(player->getBoundingBox().getMinX(), 20.0f), 0)));
		break;
	case 'D':
		player->runAction(MoveBy::create(0.2f, Vec2(min(visibleSize.width - player->getBoundingBox().getMaxX(), 20.0f), 0)));
		break;
	}
}

//发射子弹
void Thunder::fire() {
  auto bullet = Sprite::create("bullet.png");
  bullet->setAnchorPoint(Vec2(0.5, 0.5));
  bullets.push_back(bullet);
  bullet->setPosition(player->getPosition());
  addChild(bullet, 1);
  SimpleAudioEngine::getInstance()->playEffect("music/fire.wav", false);

  // 移除飞出屏幕外的子弹
  // Todo
  //bullet->runAction(MoveBy::create(1.0f, Vec2(0, visibleSize.height)));
  bullet->runAction(Sequence::create(MoveBy::create(1.0f, Vec2(0, visibleSize.height)), CallFunc::create([this, bullet]() {
	  bullet->removeFromParentAndCleanup(true);
	  this->bullets.remove(bullet);
  }), nullptr));
}

// 切割爆炸动画帧
void Thunder::explosion() {
  // Todo
	auto texture = Director::getInstance()->getTextureCache()->addImage("explosion.png");
	explore.pushBack(SpriteFrame::createWithTexture(texture, CC_RECT_PIXELS_TO_POINTS(Rect(2, 0, 140, 140))));
	explore.pushBack(SpriteFrame::createWithTexture(texture, CC_RECT_PIXELS_TO_POINTS(Rect(210, 0, 140, 140))));
	explore.pushBack(SpriteFrame::createWithTexture(texture, CC_RECT_PIXELS_TO_POINTS(Rect(400, 0, 140, 140))));
	explore.pushBack(SpriteFrame::createWithTexture(texture, CC_RECT_PIXELS_TO_POINTS(Rect(590, 0, 140, 140))));
	explore.pushBack(SpriteFrame::createWithTexture(texture, CC_RECT_PIXELS_TO_POINTS(Rect(780, 0, 140, 140))));
	explore.pushBack(SpriteFrame::createWithTexture(texture, CC_RECT_PIXELS_TO_POINTS(Rect(2, 190, 140, 140))));
	explore.pushBack(SpriteFrame::createWithTexture(texture, CC_RECT_PIXELS_TO_POINTS(Rect(210, 190, 140, 140))));
	explore.pushBack(SpriteFrame::createWithTexture(texture, CC_RECT_PIXELS_TO_POINTS(Rect(400, 190, 140, 140))));
}

void Thunder::update(float f) {
  // 实时更新页面内陨石和子弹数量(不得删除)
  // 要求数量显示正确(加分项)
  char str[15];
  sprintf(str, "enemys: %d", enemys.size());
  enemysNum->setString(str);
  sprintf(str, "bullets: %d", bullets.size());
  bulletsNum->setString(str);

  // 飞船移动
  if (isMove)
    this->movePlane(movekey);

  static int ct = 0;
  static int dir = 4;
  ++ct;
  if (ct == 120)
    ct = 40, dir = -dir;
  else if (ct == 80) {
    dir = -dir;
    newEnemy();  // 陨石向下移动并生成新的一行(加分项)
  }
  else if (ct == 20)
    ct = 40, dir = -dir;

  //陨石左右移动
  for (Sprite* s : enemys) {
    if (s != NULL) {
      s->setPosition(s->getPosition() + Vec2(dir, 0));
    }
  }

  // 分发自定义事件
  EventCustom e("meet");
  _eventDispatcher->dispatchEvent(&e);
}

// 自定义碰撞事件
void Thunder::meet(EventCustom * event) {
  // 判断子弹是否打中陨石并执行对应操作
  // Todo
	Sprite* temp;
	bool meet = false;
	for (auto bullet = bullets.begin(); bullet != bullets.end();) {
		meet = false;
		for (auto enemy = enemys.begin(); enemy != enemys.end(); ++ enemy) {
			if ((*bullet)->getPosition().getDistance((*enemy)->getPosition()) < 25 && (*bullet) && (*enemy)) {
				temp = *enemy;
				(*bullet)->removeFromParentAndCleanup(true);
				(*enemy)->runAction(Sequence::create(Animate::create(Animation::createWithSpriteFrames(explore, 0.05f, 1)), CallFunc::create([this, temp] {
					temp->removeFromParentAndCleanup(true);
				}), nullptr));
				SimpleAudioEngine::getInstance()->playEffect("music/explore.wav", false);
				meet = true;
				bullet = bullets.erase(bullet);
				enemy = enemys.erase(enemy);
				break;
			}
		}
		if (!meet) ++bullet;
	}

  // 判断游戏是否结束并执行对应操作
  // Todo
	for (auto it = enemys.begin(); it != enemys.end(); ++it) { //判断游戏是否结束
		if (*it && (*it)->getBoundingBox().getMinY() <= player->getBoundingBox().getMaxY()) {
			temp = player;
			player->runAction(Sequence::create(Animate::create(Animation::createWithSpriteFrames(explore, 0.05f, 1)), CallFunc::create([this, temp] {
				temp->removeFromParentAndCleanup(true);
				auto gameover = Sprite::create("gameOver.png");
				gameover->setAnchorPoint(Vec2(0.5, 0.5));
				gameover->setPosition(visibleSize / 2);
				this->addChild(gameover, 1);
			}), nullptr));

			SimpleAudioEngine::getInstance()->playEffect("music/explore.wav", false);
			unschedule(schedule_selector(Thunder::update));
			this->getEventDispatcher()->removeAllEventListeners();
			return; // 游戏结束要退出循环, player已经是空指针, 不能player->getBoundingBox()
		}
	}

}

// 添加自定义监听器
void Thunder::addCustomListener() {
  // Todo
	auto meetListener = EventListenerCustom::create("meet", CC_CALLBACK_1(Thunder::meet, this));
	_eventDispatcher->addEventListenerWithFixedPriority(meetListener, 1);
}

// 添加键盘事件监听器
void Thunder::addKeyboardListener() {
  // Todo
	auto keyboardListener = EventListenerKeyboard::create();
	keyboardListener->onKeyPressed = CC_CALLBACK_2(Thunder::onKeyPressed, this);
	keyboardListener->onKeyReleased = CC_CALLBACK_2(Thunder::onKeyReleased, this);
	this->getEventDispatcher()->addEventListenerWithSceneGraphPriority(keyboardListener, this);
}

void Thunder::onKeyPressed(EventKeyboard::KeyCode code, Event* event) {
  switch (code) {
  case EventKeyboard::KeyCode::KEY_LEFT_ARROW:
  case EventKeyboard::KeyCode::KEY_CAPITAL_A:
  case EventKeyboard::KeyCode::KEY_A:
    movekey = 'A';
    isMove = true;
    break;
  case EventKeyboard::KeyCode::KEY_RIGHT_ARROW:
  case EventKeyboard::KeyCode::KEY_CAPITAL_D:
  case EventKeyboard::KeyCode::KEY_D:
    movekey = 'D';
    isMove = true;
    break;
  case EventKeyboard::KeyCode::KEY_SPACE:
    fire();
    break;
  }
}

void Thunder::onKeyReleased(EventKeyboard::KeyCode code, Event* event) {
  switch (code) {
  case EventKeyboard::KeyCode::KEY_LEFT_ARROW:
  case EventKeyboard::KeyCode::KEY_A:
  case EventKeyboard::KeyCode::KEY_CAPITAL_A:
  case EventKeyboard::KeyCode::KEY_RIGHT_ARROW:
  case EventKeyboard::KeyCode::KEY_D:
  case EventKeyboard::KeyCode::KEY_CAPITAL_D:
    isMove = false;
    break;
  }
}

// 添加触摸事件监听器
void Thunder::addTouchListener() {
  // Todo
	this->setTouchEnabled(true);
	auto touchListener = EventListenerTouchOneByOne::create();
	touchListener->onTouchBegan = CC_CALLBACK_2(Thunder::onTouchBegan, this);
	touchListener->onTouchMoved = CC_CALLBACK_2(Thunder::onTouchMoved, this);
	touchListener->onTouchEnded = CC_CALLBACK_2(Thunder::onTouchEnded, this);
	this->getEventDispatcher()->addEventListenerWithSceneGraphPriority(touchListener, this);
}

// 鼠标点击发射炮弹
bool Thunder::onTouchBegan(Touch *touch, Event *event) {
	isClick = player->getBoundingBox().containsPoint(touch->getLocation());
	fire();
    return true;
}

void Thunder::onTouchEnded(Touch *touch, Event *event) {
  isClick = false;
}

// 当鼠标按住飞船后可控制飞船移动 (加分项)
void Thunder::onTouchMoved(Touch *touch, Event *event) {
  // Todo
	if (isClick) {
		float x = touch->getDelta().x + player->getPositionX();
		if (x < 0) x = 0;
		if (x > visibleSize.width) x = visibleSize.width;
		player->setPosition(x, player->getPositionY());
	}
}
