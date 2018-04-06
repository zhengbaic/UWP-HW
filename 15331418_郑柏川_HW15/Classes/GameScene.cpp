#include "GameScene.h"
#include "json/rapidjson.h"
#include "json/document.h"
#include "json/writer.h"
#include "json/stringbuffer.h"
#include <regex>
using std::regex;
using std::match_results;
using std::regex_match;
using std::cmatch;
using namespace rapidjson;

USING_NS_CC;

cocos2d::Scene* GameScene::createScene() {
    // 'scene' is an autorelease object
    auto scene = Scene::create();

    // 'layer' is an autorelease object
    auto layer = GameScene::create();

    // add layer as a child to scene
    scene->addChild(layer);

    // return the scene
    return scene;
}

bool GameScene::init() {
    if (!Layer::init())
    {
        return false;
    }

    Size size = Director::getInstance()->getVisibleSize();
    visibleHeight = size.height;
    visibleWidth = size.width;

    score_field = TextField::create("Score", "Arial", 30);
    score_field->setPosition(Size(visibleWidth / 4, visibleHeight / 4 * 3));
	char a[100];
	sprintf(a, "%ld", Global::score);
	score_field->setString(a);
    this->addChild(score_field, 2);

    submit_button = Button::create();
    submit_button->setTitleText("Submit");
    submit_button->setTitleFontSize(30);
    submit_button->setPosition(Size(visibleWidth / 4, visibleHeight / 4));
	submit_button->addClickEventListener(CC_CALLBACK_1(GameScene::SubmitBtnClick, this));
    this->addChild(submit_button, 2);

    rank_field = TextField::create("", "Arial", 30);
    rank_field->setPosition(Size(visibleWidth / 4 * 3, visibleHeight / 4 * 3));
    this->addChild(rank_field, 2);

    rank_button = Button::create();
    rank_button->setTitleText("Rank");
    rank_button->setTitleFontSize(30);
    rank_button->setPosition(Size(visibleWidth / 4 * 3, visibleHeight / 4));
	rank_button->addClickEventListener(CC_CALLBACK_1(GameScene::RankBtnClick, this));
    this->addChild(rank_button, 2);
    return true;
}

bool GameScene::SubmitBtnClick(Ref * r) {
	HttpRequest* request = new HttpRequest();
	request->setUrl("http://localhost:8080/submit");
	request->setRequestType(HttpRequest::Type::POST);
	request->setResponseCallback(CC_CALLBACK_2(GameScene::SubmitBtnCallback, this));
	string postData = "score=" + score_field->getString();
	request->setRequestData(postData.c_str(), postData.size());

	vector<string> header;
	header.push_back("Cookie: GAMESESSIONID=" + Global::gameSessionId);
	request->setHeaders(header);

	HttpClient::getInstance()->send(request);
	request->release();
	return true;
}
bool GameScene::RankBtnClick(Ref * r) {
	HttpRequest* request = new HttpRequest();
	request->setUrl("http://localhost:8080/rank?top=10");
	request->setRequestType(HttpRequest::Type::GET);
	request->setResponseCallback(CC_CALLBACK_2(GameScene::RankBtnCallback, this));
	vector<string> header;
	header.push_back("Cookie: GAMESESSIONID=" + Global::gameSessionId);
	request->setHeaders(header);
	HttpClient::getInstance()->send(request);
	request->release();
	return true;
}
void GameScene::SubmitBtnCallback(HttpClient * sender, HttpResponse * response) {
	std::vector<char> * buffer = response->getResponseData();
	string str;
	rapidjson::Document d;
	str = Global::toString(buffer);
	d.Parse<0>(str.c_str());
	if (d.IsObject() && d.HasMember("info")) {
		Global::score = atol(d["info"].GetString());
		char a[100];
		sprintf(a, "%ld", Global::score);
		score_field->setString(a);
	}
}

void GameScene::RankBtnCallback(HttpClient * sender, HttpResponse * response)
{
	if (!response) {
		return;
	}
	if (!response->isSucceed()) {
		log("respone failed");
		return;
	}
	vector<char> *_header = response->getResponseHeader();
	vector<char> *_body = response->getResponseData();
	string header = Global::toString(_header);
	string body = Global::toString(_body);
	Document d;
	d.Parse<0>(body.c_str());

	if (d["result"].GetBool()) {
		string result = d["info"].GetString();
		for (int i = 0; i < result.length(); i++)
			if (result[i] == '|')
				result[i] = '\n';
		rank_field->setText(result);
	}
}

