#include "HelloWorldScene.h"
#include "Monster.h"
#define database UserDefault::getInstance()
#pragma execution_character_set("utf-8")
USING_NS_CC;
using namespace std;
Scene* HelloWorld::createScene()
{
    // 'scene' is an autorelease object
    auto scene = Scene::create();

    // 'layer' is an autorelease object
    auto layer = HelloWorld::create();

    // add layer as a child to scene
    scene->addChild(layer);

    // return the scene
    return scene;
}

// on "init" you need to initialize your instance
bool HelloWorld::init()
{
    //////////////////////////////
    // 1. super init first
    if ( !Layer::init() )
    {
        return false;
    }

    visibleSize = Director::getInstance()->getVisibleSize();
    origin = Director::getInstance()->getVisibleOrigin();
	//背景
	auto bg = Sprite::create("002_3.png");
	bg->setPosition(Vec2(visibleSize.width / 2 + origin.x, visibleSize.height / 2 + origin.y));
	float spy = bg->getTextureRect().getMaxY();
	bg->setScaleX(visibleSize.height / spy); //设置宽度缩放比例
	bg->setScaleY(visibleSize.height / spy); //设置高度缩放比例
	this->addChild(bg, 0);

	//创建一张贴图
	auto texture = Director::getInstance()->getTextureCache()->addImage("$lucia_forward.png");
	//从贴图中以像素单位切割，创建关键帧
	frame0 = SpriteFrame::createWithTexture(texture, CC_RECT_PIXELS_TO_POINTS(Rect(0, 0, 68, 101)));
	//使用第一帧创建精灵
	player = Sprite::createWithSpriteFrame(frame0);
	player->setPosition(Vec2(origin.x + visibleSize.width / 2,
		origin.y + visibleSize.height / 2));
	Towardright = true;
	addChild(player, 1);
	// 静态动画
	idle.reserve(1);
	idle.pushBack(frame0);

	// 攻击动画
	auto texture1 = Director::getInstance()->getTextureCache()->addImage("$lucia_2.png");
	attack.reserve(17);
	for (int i = 0; i < 17; i++) {
		auto frame = SpriteFrame::createWithTexture(texture1, CC_RECT_PIXELS_TO_POINTS(Rect(113*i,0,113,113)));
		attack.pushBack(frame);
	}
	attack.pushBack(frame0);

	// 可以仿照攻击动画
	// 死亡动画(帧数：22帧，高：90，宽：79）
	auto texture2 = Director::getInstance()->getTextureCache()->addImage("$lucia_dead.png");
	dead.reserve(22);
	for (int i = 0; i < 22; i++) {
		auto frame = SpriteFrame::createWithTexture(texture2, CC_RECT_PIXELS_TO_POINTS(Rect(79 * i, 0, 79, 90)));
		dead.pushBack(frame);
	}
	dead.pushBack(frame0);
    // Todo

	// 运动动画(帧数：8帧，高：101，宽：68）
	auto texture3 = Director::getInstance()->getTextureCache()->addImage("$lucia_forward.png");
	for (int i = 0; i < 8; i++) {
		auto frame = SpriteFrame::createWithTexture(texture3, CC_RECT_PIXELS_TO_POINTS(Rect(68 * i, 0, 68, 101)));
		run.pushBack(frame);
	}
	run.pushBack(frame0);
    // Todo


	//hp条
	Sprite* sp0 = Sprite::create("hp.png", CC_RECT_PIXELS_TO_POINTS(Rect(0, 320, 420, 47)));
	Sprite* sp = Sprite::create("hp.png", CC_RECT_PIXELS_TO_POINTS(Rect(610, 362, 4, 16)));

	//使用hp条设置progressBar
	pT = ProgressTimer::create(sp);
	pT->setScaleX(90);
	pT->setAnchorPoint(Vec2(0, 0));
	pT->setType(ProgressTimerType::BAR);
	pT->setBarChangeRate(Point(1, 0));
	pT->setMidpoint(Point(0, 1));
	pT->setPercentage(100);
	pT->setPosition(Vec2(origin.x + 14 * pT->getContentSize().width, origin.y + visibleSize.height - 2 * pT->getContentSize().height));
	addChild(pT, 3);
	sp0->setAnchorPoint(Vec2(0, 0));
	sp0->setPosition(Vec2(origin.x + pT->getContentSize().width, origin.y + visibleSize.height - sp0->getContentSize().height));
	addChild(sp0, 2);

	//倒计时
	TTFConfig ttfConfig;
	ttfConfig.fontFilePath = "fonts/arial.ttf";
	ttfConfig.fontSize = 36;
	if (!database->getBoolForKey("isExist")) {
		database->setBoolForKey("isExist", true);
		database->setStringForKey("killnum", "0");
	}
	string temp = database->getStringForKey("killnum");
	kill_num = atoi(temp.c_str());
	kill_num_label = Label::createWithTTF(ttfConfig, temp);
	kill_num_label->setPosition(Vec2(origin.x + visibleSize.width / 2,
		origin.y + visibleSize.height - kill_num_label->getContentSize().height));
	this->addChild(kill_num_label, 2);



	//按键
	auto menuLabel1 = Label::createWithTTF(ttfConfig, "W");
	auto menuLabel2 = Label::createWithTTF(ttfConfig, "S");
	auto menuLabel3 = Label::createWithTTF(ttfConfig, "A");
	auto menuLabel4 = Label::createWithTTF(ttfConfig, "D");
	auto menuLabel5 = Label::createWithTTF(ttfConfig, "Y");
	//绑定菜单事件
	auto item1 = MenuItemLabel::create(menuLabel1, CC_CALLBACK_1(HelloWorld::moveEvent, this, 'W'));
	auto item2 = MenuItemLabel::create(menuLabel2, CC_CALLBACK_1(HelloWorld::moveEvent, this, 'S'));
	auto item3 = MenuItemLabel::create(menuLabel3, CC_CALLBACK_1(HelloWorld::moveEvent, this, 'A'));
	auto item4 = MenuItemLabel::create(menuLabel4, CC_CALLBACK_1(HelloWorld::moveEvent, this, 'D'));
	auto item5 = MenuItemLabel::create(menuLabel5, CC_CALLBACK_1(HelloWorld::actionEvent, this, 'Y'));
	//位置
	item3->setPosition(Vec2(origin.x + item3->getContentSize().width * 2, origin.y + item3->getContentSize().height * 2));
	item4->setPosition(Vec2(item3->getPosition().x + 3 * item4->getContentSize().width, item3->getPosition().y));
	item2->setPosition(Vec2(item3->getPosition().x + 1.5*item2->getContentSize().width, item3->getPosition().y));
	item1->setPosition(Vec2(item2->getPosition().x, item2->getPosition().y + item1->getContentSize().height));
	item5->setPosition(Vec2(origin.x + visibleSize.width - item5->getContentSize().width * 5, item3->getPosition().y));

	auto menu = Menu::create(item1, item2, item3, item4, item5, NULL);
	menu->setPosition(Vec2(0, 0));
	this->addChild(menu, 2);

	schedule(schedule_selector(HelloWorld::createMonster), 3.0f, kRepeatForever, 0);  //定时产生怪物
	schedule(schedule_selector(HelloWorld::hitByMonster), 0.05f, kRepeatForever, 0);  //定时检测碰撞

	TMXTiledMap* tmx = TMXTiledMap::create("map.tmx");
	tmx->setPosition(visibleSize.width / 2, visibleSize.height / 2);
	tmx->setAnchorPoint(Vec2(0.5, 0.5));
	tmx->setScale(visibleSize.height / tmx->getContentSize().height);
	this->addChild(tmx, 0);
	return true;
}

void HelloWorld::moveEvent(Ref *, char c)
{
	Animate* runkAnimate = Animate::create(Animation::createWithSpriteFrames(run, 0.05f, 1));
	if(pT->getPercentage() && player->getSpriteFrame() == frame0){
		auto nowPos = player->getPosition();
		switch (c) {
		case 'W':
			//Animate* runkAnimate = Animate::create(Animation::createWithSpriteFrames(run, 0.05f, 1));
			player->runAction(Spawn::create(runkAnimate, MoveBy::create(0.45f, Vec2(0, min(visibleSize.height - nowPos.y - player->getContentSize().height / 2, 50.0f))), nullptr));
			break;
		case 'A':
			if (Towardright == true) {
				Towardright = false;
				player->setFlipX(true);
			}
			//Animate* runkAnimate = Animate::create(Animation::createWithSpriteFrames(run, 0.05f, 1));
			player->runAction(Spawn::create(runkAnimate, MoveBy::create(0.45f, Vec2(-min(nowPos.x - player->getContentSize().width / 2, 50.0f), 0)), nullptr));
			break;
		case 'S':
			player->runAction(Spawn::create(runkAnimate, MoveBy::create(0.45f, Vec2(0, -min(nowPos.y - player->getContentSize().height / 2, 50.0f))), nullptr));
			break;
		case 'D':
			if (Towardright == false) {
				Towardright = true;
				player->setFlipX(false);
			}
			player->runAction(Spawn::create(runkAnimate, MoveBy::create(0.45f, Vec2(min(visibleSize.width - nowPos.x - player->getContentSize().width / 2, 50.0f), 0)), nullptr));
			break;
		}
	}
}

void HelloWorld::actionEvent(Ref *, char c)
{
	Animate* deadAnimate = Animate::create(Animation::createWithSpriteFrames(dead, 0.1f, 1));
	Animate* attackAnimate = Animate::create(Animation::createWithSpriteFrames(attack, 0.1f, 1));
	bool deading = dead.contains(player->getSpriteFrame()) && player->getSpriteFrame() != frame0;
	bool attacking = attack.contains(player->getSpriteFrame()) && player->getSpriteFrame() != frame0;
	if (c == 'X' && pT->getPercentage() > 0) {
		if (deading == false) {
			if (pT->getPercentage() <= 20)
				dead.popBack();
			deadAnimate = Animate::create(Animation::createWithSpriteFrames(dead, 0.1f, 1));
			player->runAction(deadAnimate);
			pT->runAction(CCProgressTo::create(2, pT->getPercentage() - 20));
		}
	}
	if (c == 'Y' && pT->getPercentage() > 0) {
		if (attacking == false && deading == false) {
			player->stopAllActions();
			attackAnimate = Animate::create(Animation::createWithSpriteFrames(attack, 0.05f, 1));
			player->runAction(attackAnimate);
			hitMonster();
		}
	}
	/*Animate* deadAnimate;
	Animate* attackAnimate;
	if (pT->getPercentage()&&player->getSpriteFrame() == frame0)  {
		switch (c) {
		case 'X':
			if (pT->getPercentage() <= 20)
				dead.popBack();
			deadAnimate = Animate::create(Animation::createWithSpriteFrames(dead, 0.1f, 1));
			player->runAction(deadAnimate);
			pT->runAction(CCProgressTo::create(2, pT->getPercentage() - 20));
			break;
		case 'Y':
			attackAnimate = Animate::create(Animation::createWithSpriteFrames(attack, 0.1f, 1));
			player->runAction(attackAnimate);
			hitMonster();
			break;
		}
	}*/
}

void HelloWorld::createMonster(float t) {
	auto temp = Factory::getInstance()->createMonster();
	temp->setPosition(random(origin.x, visibleSize.width), random(origin.y, visibleSize.height));
	this->addChild(temp, 1);
	Factory::getInstance()->moveMonster(player->getPosition(), 3.0f);
}

void HelloWorld::hitMonster() {
	auto factory = Factory::getInstance();
	Sprite* MonsterWhenAttack = factory->haveMonster(player->getBoundingBox());
	if (MonsterWhenAttack != nullptr) {
		kill_num++;
		factory->removeMonster(MonsterWhenAttack);
		if (pT->getPercentage() != 100)
			pT->runAction(CCProgressTo::create(1.8f, pT->getPercentage() + 20));
		char ct[10];
		_itoa(kill_num, ct, 10);
		kill_num_label->setString(ct);
		database->setStringForKey("killnum", string(ct));
	}
}

void HelloWorld::hitByMonster(float dt) {
	bool deading = dead.contains(player->getSpriteFrame()) && player->getSpriteFrame() != frame0;
	bool attacking = attack.contains(player->getSpriteFrame()) && player->getSpriteFrame() != frame0;
	if (deading == false && attacking == false) {
		auto fac = Factory::getInstance();
		Sprite* collison = fac->collider(player->getBoundingBox());
		if (collison != nullptr) {
			player->stopAllActions();
			fac->removeMonster(collison);
			this->actionEvent(this, 'X');
		}
	}
}
