#include "GameSence.h"

USING_NS_CC;

Scene* GameSence::createScene()
{
	// 'scene' is an autorelease object
	auto scene = Scene::create();

	// 'layer' is an autorelease object
	auto layer = GameSence::create();

	// add layer as a child to scene
	scene->addChild(layer);

	// return the scene
	return scene;
}

// on "init" you need to initialize your instance
bool GameSence::init()
{

	if (!Layer::init())
	{
		return false;
	}

	//add touch listener
	EventListenerTouchOneByOne* listener = EventListenerTouchOneByOne::create();
	listener->setSwallowTouches(true);
	listener->onTouchBegan = CC_CALLBACK_2(GameSence::onTouchBegan, this);
	Director::getInstance()->getEventDispatcher()->addEventListenerWithSceneGraphPriority(listener, this);

	//背景
	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();
	auto background = Sprite::create("level-background-0.jpg");
	background->setPosition(Vec2(visibleSize.width / 2 + origin.x, visibleSize.height / 2 + origin.y));
	float ScaleTimes = visibleSize.height / background->getContentSize().height;//计算缩放倍数
	background->setScale(ScaleTimes);
	this->addChild(background, 0);
	//stone layer
	stoneLayer = Layer::create();
	stoneLayer->ignoreAnchorPointForPosition(false);
	stoneLayer->setAnchorPoint(Vec2::ZERO);
	stoneLayer->setPosition(Vec2::ZERO);
	stone = Sprite::create("stone.png");
	stone->setPosition(Vec2(560, 480));
	stoneLayer->addChild(stone);
	this->addChild(stoneLayer, 1);

	// shoot及其点击事件
	auto label = Label::createWithTTF("Shoot", "fonts/msyh.ttf", 65); //label
	auto shootButton = MenuItemLabel::create(label, CC_CALLBACK_1(GameSence::shootMenuCallback, this)); //menuitemlabel
	Menu* shoot = Menu::create(shootButton, NULL); //menu
	shoot->setPosition(Vec2(780, 480));
	stoneLayer->addChild(shoot);

	// mouseLayer
	mouseLayer = Layer::create();
	mouseLayer->ignoreAnchorPointForPosition(false);
	mouseLayer->setAnchorPoint(Vec2::ZERO);
	mouseLayer->setPosition(Vec2::ZERO);
	this->addChild(mouseLayer, 1);

	mouse = Sprite::createWithSpriteFrameName("gem-mouse-0.png");
	Animate* mouseAnimate = Animate::create(AnimationCache::getInstance()->getAnimation("mouseAnimation"));
	mouse->runAction(RepeatForever::create(mouseAnimate));
	mouse->setPosition(visibleSize.width / 2, visibleSize.height/2);
	//toPos = Vec2(visibleSize.width / 2, 0);
	mouseLayer->addChild(mouse, 2);


	return true;
}

bool GameSence::onTouchBegan(Touch *touch, Event *unused_event) {

	auto location = touch->getLocation();
	auto mouselocation = mouse->getPosition();
	Size visibleSize = Director::getInstance()->getVisibleSize();
	
	cheese = Sprite::create("cheese.png");
	cheese->setPosition(location.x, location.y);
	mouseLayer->addChild(cheese);

	auto moveto = MoveTo::create(2.0, Vec2((int)location.x, (int)location.y));
	mouse->runAction(moveto);
	cheese->runAction(Sequence::create(ScaleTo::create(2.0, 1.0), FadeOut::create(1.0), nullptr));
	return true;
}
void GameSence::shootMenuCallback(Ref * pSender) {
	auto stonelocation = stone->getPosition();
	auto mouselocation = mouse->getPosition();
	Size visibleSize = Director::getInstance()->getVisibleSize();

	auto shootstone = Sprite::create("stone.png");
	shootstone->setPosition(stone->getPosition());
	this->addChild(shootstone, 1);
	auto seq = Sequence::create(MoveTo::create(1.5, Vec2(mouselocation.x, mouselocation.y)), FadeOut::create(0.5), nullptr);
	shootstone->runAction(seq);

	auto diamond = Sprite::create("diamond.png");
	diamond->setPosition(mouselocation);
	mouseLayer->addChild(diamond, 1);

	Vec2 randomloc = Vec2((int)(CCRANDOM_0_1() * 960), (int)(CCRANDOM_0_1() * 420));
	auto mousemoveto = JumpTo::create(1.5, randomloc, 50, 4);
	mouse->runAction(mousemoveto);
}