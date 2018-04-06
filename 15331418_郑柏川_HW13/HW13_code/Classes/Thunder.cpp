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

  isMove = false;  // �Ƿ����ɴ�
  visibleSize = Director::getInstance()->getVisibleSize();

  // ��������
  auto bgsprite = Sprite::create("bg.jpg");
  bgsprite->setPosition(visibleSize / 2);
  bgsprite->setScale(visibleSize.width / bgsprite->getContentSize().width,
                     visibleSize.height / bgsprite->getContentSize().height);
  this->addChild(bgsprite, 0);

  // �����ɴ�
  player = Sprite::create("player.png");
  player->setAnchorPoint(Vec2(0.5, 0.5));
  player->setPosition(visibleSize.width / 2, player->getContentSize().height);
  player->setName("player");
  this->addChild(player, 1);

  // ��ʾ��ʯ���ӵ�����
  enemysNum = Label::createWithTTF("enemys: 0", "fonts/arial.TTF", 20);
  enemysNum->setColor(Color3B(255, 255, 255));
  enemysNum->setPosition(50, 60);
  this->addChild(enemysNum, 3);
  bulletsNum = Label::createWithTTF("bullets: 0", "fonts/arial.TTF", 20);
  bulletsNum->setColor(Color3B(255, 255, 255));
  bulletsNum->setPosition(50, 30);
  this->addChild(bulletsNum, 3);

  addEnemy(5);        // ��ʼ����ʯ
  preloadMusic();     // Ԥ��������
  playBgm();          // ���ű�������
  explosion();        // ������ը֡����

  // ��Ӽ�����
  addTouchListener();
  addKeyboardListener();
  addCustomListener();

  // ������
  schedule(schedule_selector(Thunder::update), 0.04f, kRepeatForever, 0);

  return true;
}

//Ԥ���������ļ�
void Thunder::preloadMusic() {
  // Todo
	auto audio = SimpleAudioEngine::getInstance();
	audio->preloadBackgroundMusic("music/bgm.mp3");
	audio->preloadEffect("music/explore.wav");
	audio->preloadEffect("music/fire.wav");
}

//���ű�������
void Thunder::playBgm() {
  // Todo
	SimpleAudioEngine::getInstance()->playBackgroundMusic("music/bgm.mp3", true);
}

//��ʼ����ʯ
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

// ��ʯ�����ƶ��������µ�һ��(�ӷ���)
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

// �ƶ��ɴ�
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

//�����ӵ�
void Thunder::fire() {
  auto bullet = Sprite::create("bullet.png");
  bullet->setAnchorPoint(Vec2(0.5, 0.5));
  bullets.push_back(bullet);
  bullet->setPosition(player->getPosition());
  addChild(bullet, 1);
  SimpleAudioEngine::getInstance()->playEffect("music/fire.wav", false);

  // �Ƴ��ɳ���Ļ����ӵ�
  // Todo
  //bullet->runAction(MoveBy::create(1.0f, Vec2(0, visibleSize.height)));
  bullet->runAction(Sequence::create(MoveBy::create(1.0f, Vec2(0, visibleSize.height)), CallFunc::create([this, bullet]() {
	  bullet->removeFromParentAndCleanup(true);
	  this->bullets.remove(bullet);
  }), nullptr));
}

// �иը����֡
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
  // ʵʱ����ҳ������ʯ���ӵ�����(����ɾ��)
  // Ҫ��������ʾ��ȷ(�ӷ���)
  char str[15];
  sprintf(str, "enemys: %d", enemys.size());
  enemysNum->setString(str);
  sprintf(str, "bullets: %d", bullets.size());
  bulletsNum->setString(str);

  // �ɴ��ƶ�
  if (isMove)
    this->movePlane(movekey);

  static int ct = 0;
  static int dir = 4;
  ++ct;
  if (ct == 120)
    ct = 40, dir = -dir;
  else if (ct == 80) {
    dir = -dir;
    newEnemy();  // ��ʯ�����ƶ��������µ�һ��(�ӷ���)
  }
  else if (ct == 20)
    ct = 40, dir = -dir;

  //��ʯ�����ƶ�
  for (Sprite* s : enemys) {
    if (s != NULL) {
      s->setPosition(s->getPosition() + Vec2(dir, 0));
    }
  }

  // �ַ��Զ����¼�
  EventCustom e("meet");
  _eventDispatcher->dispatchEvent(&e);
}

// �Զ�����ײ�¼�
void Thunder::meet(EventCustom * event) {
  // �ж��ӵ��Ƿ������ʯ��ִ�ж�Ӧ����
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

  // �ж���Ϸ�Ƿ������ִ�ж�Ӧ����
  // Todo
	for (auto it = enemys.begin(); it != enemys.end(); ++it) { //�ж���Ϸ�Ƿ����
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
			return; // ��Ϸ����Ҫ�˳�ѭ��, player�Ѿ��ǿ�ָ��, ����player->getBoundingBox()
		}
	}

}

// ����Զ��������
void Thunder::addCustomListener() {
  // Todo
	auto meetListener = EventListenerCustom::create("meet", CC_CALLBACK_1(Thunder::meet, this));
	_eventDispatcher->addEventListenerWithFixedPriority(meetListener, 1);
}

// ��Ӽ����¼�������
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

// ��Ӵ����¼�������
void Thunder::addTouchListener() {
  // Todo
	this->setTouchEnabled(true);
	auto touchListener = EventListenerTouchOneByOne::create();
	touchListener->onTouchBegan = CC_CALLBACK_2(Thunder::onTouchBegan, this);
	touchListener->onTouchMoved = CC_CALLBACK_2(Thunder::onTouchMoved, this);
	touchListener->onTouchEnded = CC_CALLBACK_2(Thunder::onTouchEnded, this);
	this->getEventDispatcher()->addEventListenerWithSceneGraphPriority(touchListener, this);
}

// ����������ڵ�
bool Thunder::onTouchBegan(Touch *touch, Event *event) {
	isClick = player->getBoundingBox().containsPoint(touch->getLocation());
	fire();
    return true;
}

void Thunder::onTouchEnded(Touch *touch, Event *event) {
  isClick = false;
}

// ����갴ס�ɴ���ɿ��Ʒɴ��ƶ� (�ӷ���)
void Thunder::onTouchMoved(Touch *touch, Event *event) {
  // Todo
	if (isClick) {
		float x = touch->getDelta().x + player->getPositionX();
		if (x < 0) x = 0;
		if (x > visibleSize.width) x = visibleSize.width;
		player->setPosition(x, player->getPositionY());
	}
}
