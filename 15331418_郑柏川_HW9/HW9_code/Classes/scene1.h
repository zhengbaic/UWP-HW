#pragma once
#ifndef __NEWS_H__
#define __NEWS_H__

#include "cocos2d.h"

class NewS : public cocos2d::Scene
{
public:
	static cocos2d::Scene* createScene();

	virtual bool init();

	// a selector callback

	// implement the "static create()" method manually
	CREATE_FUNC(NewS);
};

#endif // __NEWS_H__
