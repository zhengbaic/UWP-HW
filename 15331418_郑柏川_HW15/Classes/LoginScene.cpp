#include "LoginScene.h"
#include "cocostudio/CocoStudio.h"
#include "json/rapidjson.h"
#include "json/document.h"
#include "json/writer.h"
#include "json/stringbuffer.h"
#include "Global.h"
#include "GameScene.h"
#include <regex>
#define database UserDefault::getInstance()
using std::to_string;
using std::regex;
using std::match_results;
using std::regex_match;
using std::cmatch;
using namespace rapidjson;
USING_NS_CC;

using namespace cocostudio::timeline;

#include "json/document.h"
#include "json/writer.h"
#include "json/stringbuffer.h"
using namespace  rapidjson;

Scene* LoginScene::createScene()
{
    // 'scene' is an autorelease object
    auto scene = Scene::create();
    
    // 'layer' is an autorelease object
    auto layer = LoginScene::create();

    // add layer as a child to scene
    scene->addChild(layer);

    // return the scene
    return scene;
}





// on "init" you need to initialize your instance
bool LoginScene::init()
{
    // 1. super init first
    if ( !Layer::init() )
    {
        return false;
    }

    Size size = Director::getInstance()->getVisibleSize();
    visibleHeight = size.height;
    visibleWidth = size.width;

    textField = TextField::create("Player Name", "Arial", 30);
    textField->setPosition(Size(visibleWidth / 2, visibleHeight / 4 * 3));
    this->addChild(textField, 2);

    auto button = Button::create();
	button->addClickEventListener(CC_CALLBACK_1(LoginScene::LoginBtnClick, this));
    button->setTitleText("Login");
    button->setTitleFontSize(30);
    button->setPosition(Size(visibleWidth / 2, visibleHeight / 2));
    this->addChild(button, 2);
	/*if (database->getStringForKey("username") != "") {
		textField->setString(database->getStringForKey("username"));
		LoginBtnClick(nullptr);
	}*/
    return true;
}

void LoginScene::LoginBtnClick(Ref * r) {
	Login(textField->getString());
}

void LoginScene::LoginCallback(HttpClient * sender, HttpResponse * res) {
	if (res->isSucceed()) {
		string resHeader(res->getResponseHeader()->begin(), res->getResponseHeader()->end());
		Global::gameSessionId = Global::getSessionIdFromHeader(resHeader);
		database->setStringForKey("username", textField->getString());
		CCDirector::sharedDirector()->replaceScene(GameScene::createScene());
	}
}

void LoginScene::Login(string u)
{
	if (textField->getString() != "") {
		string data = "username=" + textField->getString();
		HttpRequest* req = new HttpRequest();
		req->setRequestType(HttpRequest::Type::POST);
		req->setUrl("localhost:8080/login");
		req->setRequestData(data.c_str(), data.size());
		req->setResponseCallback(CC_CALLBACK_2(LoginScene::LoginCallback, this));
		HttpClient::getInstance()->send(req);
		req->release();
	}
}

