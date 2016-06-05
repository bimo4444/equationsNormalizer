# equationsNormalizer
represents normalized equation:
- equates it to zero
- sums up identiс variables
- opens nested brackets

intelligence:
- don't understand multiplication and division
- skips all signs +/- exclude last before a variable //errors like (1h+-1g) will be proceed as h-g
- all next to first letter char is a variable        //errors like (1h4j + 1h4j) will be proceed as 2h4g
- point before letter or last point causes wrong format exception  //1.v || 3.u || 3.
