const Discord = require('discord.js');
const bot = new Discord.Client();

const token = 'NzM1MTk2Njg4MjgwNTg0Mjky.XxdgDw.hBgN0QnNxp7NLAg7YMpheNUmIBk';

bot.on('ready', () =>{  
    console.log('This bot is online');
})

bot.on('message', msg=>{
    if(msg.content ===  "Is use online?"){
        msg.reply('i don`t know my developer <@274853020347400192> still develop me');
    }

    if(msg.content === "Is SoftwareRat online?"){
        msg.reply('<@274853020347400192> are you online?');
	}
})

bot.login(token);